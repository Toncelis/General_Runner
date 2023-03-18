using System.Collections.Generic;
using System.Linq;
using DataTypes;
using DefaultNamespace.Signals;
using Services;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace DefaultNamespace.Managers {
    public class RoomProgressionManager : MonoBehaviour {
        [SerializeField] private StartSettings StartSettings;
        
        [PropertySpace(4),FoldoutGroup("ExternalDependencies")]
        [SerializeField] private WorldTilesManager TilesManager;
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

        [PropertySpace(4),FoldoutGroup("Signals for service")]
        [SerializeField] private CollectablesSignal LockCollectableSignal;
        [FoldoutGroup("Signals for service")]
        [SerializeField] private CollectablesSignal UnlockCollectableSignal;
        
        private CollectablesService collectablesService => ServiceLibrary.GetService<CollectablesService>();
        
        
        private readonly Dictionary<RoomChangingRule, RoomGoal> _roomGoals = new();
        private RoomSettings _currentRoom;
        private RoomChangingRule _activeChangeRule = null;

        #region Tracking room progress 
        private void OnEnable() {
            collectablesService.RegisterDependencies(PickupSignal, LockCollectableSignal, UnlockCollectableSignal);
            PickupSignal.RegisterResponse(OnCollectablesSignal);
            PrepareRoom(StartSettings.startingRoom);
        }

        private void OnDisable() {
            PickupSignal.UnregisterResponse(OnCollectablesSignal);
        }

        private void OnCollectablesSignal(CollectableTypesEnum type) {
            if (collectablesService.IsCollectableLocked(type)) {
                return;
            }

            RunRoomFinishedCheck(type);
        }
        
        private void PrepareRoom(RoomSettings room) {
            foreach (var goal in _roomGoals) {
                goal.Value.Hide(() => _roomGoals.Remove(goal.Key));
            }
            _roomGoals.Clear();

            int exitOptionsCount = room.nextRoomVariants.Count;
            float offset = (exitOptionsCount - 1) * (RoomGoalSpace + RoomGoalWidth);
            offset /= -2;

            for (int i = 0; i < exitOptionsCount; i++) {
                var roomGoal = Instantiate(RoomGoalPrefab, UIPanel).GetComponent<RoomGoal>();
                roomGoal.Setup(Vector3.right * offset, room.nextRoomVariants[i]);
                offset += RoomGoalWidth + RoomGoalSpace;
                _roomGoals.Add(room.nextRoomVariants[i], roomGoal);
            }

            _currentRoom = room;

            collectablesService.RefreshCollectables(_currentRoom);
        }
        
        private void RunRoomFinishedCheck(CollectableTypesEnum collectedType) {
            foreach (var finishRule in _currentRoom.nextRoomVariants) {
                if (finishRule.requirements.All(requirement =>
                        collectablesService.GetCollectedAmount(requirement.collectableType) >= requirement.requiredAmount
                    )
                   ) {
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
                progression += (float)collectablesService.GetCollectedAmount(type) / amount;
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
            TilesManager.NormalizeAndLoadTile(rule.changerTile, () => PrepareRoom(rule.nextRoom));
            TilesManager.NormalizeAndLoadTile(rule.nextRoom.startRoadTile);
            _activeChangeRule = null;
        }
        #endregion
    }
}