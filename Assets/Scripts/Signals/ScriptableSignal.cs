using System;
using System.Collections.Generic;
using Extensions;
using UnityEngine;

namespace DefaultNamespace.Signals {
    [CreateAssetMenu(menuName = "SoSignals/Simple", order = 0, fileName = "SimpleSoSignal")]
    public class ScriptableSignal : ScriptableObject {
        private readonly List<Action> _responses = new();
        
        public void RegisterResponse(Action response) {
            _responses.AddIfNewAndNotNull(response);
        }

        public void UnregisterResponse(Action response) {
            _responses.Remove(response);
        }

        public void Fire() {
            foreach (var response in _responses) {
                response();
            }
        }
    }
}