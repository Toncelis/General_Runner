using System.Collections;
using UnityEngine;

namespace DefaultNamespace.Managers {
    public class CoroutineManager : MonoBehaviour {
        private static CoroutineManager _coroutineManagerSingleton;
        public static CoroutineManager CoroutineManagerSingleton => _coroutineManagerSingleton;

        private void OnEnable() {
            if (CoroutineManagerSingleton == null) {
                _coroutineManagerSingleton = this;
            } else {
                Destroy(gameObject);
            }
        }

        public Coroutine StartRoutine (IEnumerator coroutine) {
            return StartCoroutine(coroutine);
        }

        public void StopRoutine(Coroutine coroutine) {
            StopCoroutine(coroutine);
        }
    }
}