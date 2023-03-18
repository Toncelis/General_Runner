using System;
using System.Collections.Generic;
using DataTypes;
using Extensions;
using UnityEngine;

namespace DefaultNamespace.Signals {
    [CreateAssetMenu(menuName = "SoSignals/Collectables", order = 0, fileName = "CollectableSoSignal")]
    public class CollectablesSignal : ScriptableObject {
        private List<Action<CollectableTypesEnum>> _responses = new();

        public void RegisterResponse(Action<CollectableTypesEnum> response) {
            _responses.AddIfNewAndNotNull(response);
        }

        public void UnregisterResponse(Action<CollectableTypesEnum> response) {
            _responses.Remove(response);
        }

        public void Fire(CollectableTypesEnum type) {
            foreach (var response in _responses) {
                response(type);
            }
        }
    }
}