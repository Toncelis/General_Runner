using System;
using System.Collections.Generic;
using System.Linq;
using DataTypes;
using DefaultNamespace.Services;
using DefaultNamespace.Signals;
using DG.Tweening;
using Services;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace DefaultNamespace.Managers {
    public class RoomProgressionManager : MonoBehaviour {
        [SerializeField] private StartSettings StartSettings;
        
        [PropertySpace(4),FoldoutGroup("ExternalDependencies")]
        [FoldoutGroup("ExternalDependencies")]
        [SerializeField] private CollectablesSignal PickupSignal;
        
        [PropertySpace(4),FoldoutGroup("UI")]
        [SerializeField] private RectTransform UIPanel;
        [FoldoutGroup("UI")]
        [SerializeField] private GameObject RoomGoalPrefab;
        [FoldoutGroup("UI")]
        [SerializeField] private float RoomGoalWidth;
        [FoldoutGroup("UI")]
        [SerializeField] private float RoomGoalSpace;

        private CollectablesService collectablesService => ServiceLibrary.GetService<CollectablesService>();
        private WorldTilesService tilesService => ServiceLibrary.GetService<WorldTilesService>();
        
        private readonly Dictionary<RoomChangingRule, RoomGoal> _roomGoals = new();
        private RoomSettings _currentRoom;
        private RoomChangingRule _activeChangeRule = null;
        private RoomSettings _nextRoom;

        #region Tracking room progress 
        private void Start() {
            PickupSignal.RegisterResponse(OnCollectablesSignal);
            _nextRoom = StartSettings.startingRoom;
            PrepareRoom();
        }

        private void OnDisable() {
            PickupSignal.UnregisterResponse(OnCollectablesSignal);
        }

        private void OnCollectablesSignal(CollectableTypesEnum type) {
            if (collectablesService.CollectionLocked) {
                return;
            }

            RunRoomFinishedCheck(type);
        }
        
        private void PrepareRoom() {
            foreach (var goal in _roomGoals) {
                goal.Value.Hide(() => _roomGoals.Remove(goal.Key));
            }
            _roomGoals.Clear();
            collectablesService.RefreshCollectables(_nextRoom);

            DOTween.To(() => RenderSettings.fogColor, color => RenderSettings.fogColor = color, _nextRoom.fogColor, 2f);
            DOTween.To(() => RenderSettings.fogStartDistance, f => RenderSettings.fogStartDistance = f, _nextRoom.fogMinDistance, 2f);
            DOTween.To(() => RenderSettings.fogEndDistance, f => RenderSettings.fogEndDistance = f, _nextRoom.fogMaxDistance, 2f);
            
            int exitOptionsCount = _nextRoom.nextRoomVariants.Count;
            float offset = (exitOptionsCount - 1) * (RoomGoalSpace + RoomGoalWidth);
            offset /= -2;

            for (int i = 0; i < exitOptionsCount; i++) {
                var roomGoal = Instantiate(RoomGoalPrefab, UIPanel).GetComponent<RoomGoal>();
                roomGoal.Setup(Vector3.right * offset, _nextRoom.nextRoomVariants[i]);
                offset += RoomGoalWidth + RoomGoalSpace;
                _roomGoals.Add(_nextRoom.nextRoomVariants[i], roomGoal);
            }

            _currentRoom = _nextRoom;
            _nextRoom = null;
            _currentRoom.behaviour?.Play();
        }
        
        private void RunRoomFinishedCheck(CollectableTypesEnum collectedType) {
            foreach (var finishRule in _currentRoom.nextRoomVariants) {
                if (finishRule.requirements.All(requirement =>
                        collectablesService.GetCollectedAmount(requirement.collectableType) >= requirement.requiredAmount
                    )
                   ) {
                    Debug.Log("room finished");
                    FinishRoom(finishRule);
                    return;
                }
            }

            foreach (var finishRule in _currentRoom.nextRoomVariants) {
                if (finishRule.requirements.Any(rule => rule.collectableType == collectedType)) {
                    UpdateRuleIcon(finishRule);
                }
            }
        }
        
        private void FinishRoom(RoomChangingRule rule) {
            foreach (var ruleToGoal in _roomGoals) {
                var (goalRule, goal) = ruleToGoal;
                if (goalRule == rule) {
                    goal.ChangeFilling(0);
                    goal.ChangePosition(0);
                    goal.HideSubGoals();
                    _activeChangeRule = rule;
                } else {
                    goal.Hide();
                }
            }
            collectablesService.LockCollection();
        }
        
        private void UpdateRuleIcon(RoomChangingRule rule) {
            float progression = 0;
            int requirementsCount = 0;
            foreach (var requirement in rule.requirements) {
                requirementsCount++;
                var type = requirement.collectableType;
                var amount = requirement.requiredAmount;
                progression += Mathf.Min((float)collectablesService.GetCollectedAmount(type) / amount, 1);
            }

            var progress = Mathf.Min(1, progression / requirementsCount);
            _roomGoals[rule].ChangeFilling(1 - progress);
        }
        #endregion
        
        #region Loading new room
        private void Update() {
            if (_activeChangeRule != null) {
                if (Input.GetKeyDown(KeyCode.LeftControl)) {
                    LoadNextRoom(_activeChangeRule);
                }
            }
        }

        private void LoadNextRoom(RoomChangingRule rule) {
            _roomGoals.Values.ForEach(goal => goal.Hide());
            Action onRoomChange;
            _nextRoom = rule.nextRoom;
            onRoomChange = PrepareRoom;
            if (_currentRoom.behaviour != null) {
                onRoomChange += _currentRoom.behaviour.Stop;
            }
            tilesService.NormalizeAndLoadTile(rule.changerTile, onRoomChange);
            tilesService.SetVisibilityDistance(rule.nextRoom.generationDistance);
            tilesService.NormalizeAndLoadTile(rule.nextRoom.startRoadTile);
            _activeChangeRule = null;
        }
        #endregion
    }
}