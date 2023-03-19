using System.Collections.Generic;
using DataTypes;
using DefaultNamespace.Signals;
using Sirenix.Utilities;
using UnityEngine;

namespace Services {
    public class CollectablesService : Service {
        private readonly Dictionary<CollectableTypesEnum, int> _currentCollection = new ();
        private readonly Dictionary<CollectableTypesEnum, int> _maxCollection = new();
        private readonly Dictionary<CollectableTypesEnum, CollectableData> _collectables = new();

        private CollectablesSignal _pickupSignal = null;
        private CollectablesSignal _lockCollectableSignal = null;
        private CollectablesSignal _unlockCollectableSignal = null;

        private bool _lockCollection = false;

        public override void SetupService(ISourceOfServiceDependencies source) {
            _pickupSignal = source.collectablePickup;
            _lockCollectableSignal = source.collectableLocked;
            _unlockCollectableSignal = source.collectableUnlocked;

            source.allCollectables.ForEach(RegisterCollectable);
        }

        public void RefreshCollectables(RoomSettings currentRoom) {
            FreeCollectables();
            SetupCollections(currentRoom);
        }

        private void FreeCollectables() {
            if (_currentCollection.Count > 0) {
                foreach (var type in _currentCollection.Keys) {
                    _unlockCollectableSignal.Fire(type);
                }
            }
            _currentCollection.Clear();
            _maxCollection.Clear(); 
            
            _lockCollection = false;
        }

        private void SetupCollections(RoomSettings currentRoom) {
            foreach (var changingRule in currentRoom.nextRoomVariants) {
                foreach (var requirement in changingRule.requirements) {
                    var type = requirement.collectableType;
                    var requiredAmount = requirement.requiredAmount;
                    if (_currentCollection.ContainsKey(type)) {
                        _maxCollection[type] = Mathf.Max(requiredAmount, _maxCollection[type]);
                    } else {
                        _currentCollection.Add(type, 0);
                        _maxCollection.Add(type, requiredAmount);
                        _unlockCollectableSignal.Fire(type);
                    }
                }
            }
        }

        public void LockCollection() {
            foreach (var type in _currentCollection.Keys) {
                if (!IsCollectableLocked(type)) {
                    _lockCollectableSignal.Fire(type);
                }
            }
            
            _lockCollection = true;
        }
        
        public bool IsCollectableLocked(CollectableTypesEnum type) {
            if (!_maxCollection.ContainsKey(type)) {
                return true;
            }
            return _lockCollection || _maxCollection[type] <= _currentCollection[type];
        }

        public void PickUpCollectable(CollectableTypesEnum type) {
            _currentCollection[type]++;
            _pickupSignal.Fire(type);
            if (_currentCollection[type] == _maxCollection[type]) {
                _lockCollectableSignal.Fire(type);
            }
        }

        private void RegisterCollectable(CollectableData collectable) {
            if (_collectables.ContainsKey(collectable.type)) {
                return;
            }
            
            _collectables.Add(collectable.type, collectable);
        }
        
        public CollectableData GetCollectableInfo(CollectableTypesEnum type) {
            return _collectables[type];
        }

        public int GetCollectedAmount(CollectableTypesEnum type) {
            return _currentCollection.ContainsKey(type) ? _currentCollection[type] : 0;
        }
    }
}