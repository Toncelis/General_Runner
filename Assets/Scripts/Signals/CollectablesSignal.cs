using System;
using System.Collections.Generic;
using Extensions;
using UnityEngine;

namespace DefaultNamespace.Signals {
    [CreateAssetMenu(menuName = "SoSignals/Collectables", order = 0, fileName = "CollectableSoSignal")]
    public class CollectablesSignal : ScriptableObject {
        private List<Action<CollectableTypes>> _responses = new();
        
        public void RegisterResponse(Action<CollectableTypes> response) {
            _responses.AddIfNewAndNotNull(response);
        }

        public void UnregisterResponse(Action<CollectableTypes> response) {
            _responses.Remove(response);
        }

        public void Fire(CollectableTypes type) {
            foreach (var response in _responses) {
                response(type);
            }
        }
    }
}