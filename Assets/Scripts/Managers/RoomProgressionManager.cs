using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Signals;
using UnityEngine;

namespace DefaultNamespace.Managers {
    public class RoomProgressionManager : MonoBehaviour {
        public CollectablesSignal signal;
        public StartSettings StartSettings;
        private RoomSettings _currentRoom;
        public WorldTilesManager TilesManager;

        private Dictionary<CollectableTypes, int> _collectionProgress = new();
        private Dictionary<RoomChangingRule, RoomGoal> _roomGoals = new();

        public RectTransform UIPanel;
        public GameObject RoomGoalPrefab;
        public float RoomGoalWidth;
        public float RoomGoalSpace;

        private bool _collectablesLocked = false;

        private void OnCollectablesSignal(CollectableTypes type) {
            if (_collectablesLocked) {
                return;
            }

            _collectionProgress[type]++;
            RunRoomFinishedCheck(type);
        }

        private void RunRoomFinishedCheck(CollectableTypes collectedType) {
            foreach (var finishRule in _currentRoom.NextRooms) {
                if (finishRule.Requirements.All(rule => _collectionProgress[rule.collectable] >= rule.amount)) {
                    FinishRoom(finishRule);
                    return;
                }
            }

            foreach (var finishRule in _currentRoom.NextRooms) {
                if (finishRule.Requirements.Any(rule => rule.collectable == collectedType)) {
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
                    TilesManager.NormalizeAndLoadTile(rule.ChangerTile, () => PrepareRoom(rule.NextRoom));
                } else {
                    goal.Hide();
                }
            }
            _collectablesLocked = true;
        }

        private void PrepareRoom(RoomSettings room) {
            foreach (var goal in _roomGoals) {
                goal.Value.Hide(() => _roomGoals.Remove(goal.Key));
            }
            _collectablesLocked = false;
            _collectionProgress.Clear();
            _roomGoals.Clear();

            int exitOptionsCount = room.NextRooms.Count;
            float offset = (exitOptionsCount - 1) * (RoomGoalSpace + RoomGoalWidth);
            offset /= -2;

            for (int i = 0; i < exitOptionsCount; i++) {
                var roomGoal = Instantiate(RoomGoalPrefab, UIPanel).GetComponent<RoomGoal>();
                roomGoal.Setup(Vector3.right * offset, room.NextRooms[i].NextRoom.RoomSprite);
                offset += RoomGoalWidth + RoomGoalSpace;
                _roomGoals.Add(room.NextRooms[i], roomGoal);

                foreach (var requirement in room.NextRooms[i].Requirements) {
                    if (!_collectionProgress.ContainsKey(requirement.collectable)) {
                        _collectionProgress.Add(requirement.collectable, 0);
                    }
                }
            }

            _currentRoom = room;
        }

        private void UpdateRuleIcon(RoomChangingRule rule) {
            float progression = 0;
            int requirementsCount = 0;
            foreach (var requirement in rule.Requirements) {
                requirementsCount++;
                var type = requirement.collectable;
                var amount = requirement.amount;
                progression += (float)_collectionProgress[type] / amount;
            }

            var progress = Mathf.Min(1, progression / requirementsCount);
            _roomGoals[rule].ChangeFilling(1 - progress);
        }

        private void OnEnable() {
            signal.RegisterResponse(OnCollectablesSignal);
            PrepareRoom(StartSettings.StartingRoom);
        }

        private void OnDisable() {
            signal.UnregisterResponse(OnCollectablesSignal);
        }
    }
}