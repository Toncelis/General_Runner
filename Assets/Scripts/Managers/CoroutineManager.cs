using System.Collections;
using UnityEngine;

namespace DefaultNamespace.Managers {
    public class CoroutineManager : MonoBehaviour {
        private static CoroutineManager _instance;
        public static CoroutineManager Instance => _instance;

        private void OnEnable() {
            if (Instance == null) {
                _instance = this;
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