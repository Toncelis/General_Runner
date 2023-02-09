using System.Collections;
using DefaultNamespace.Signals;
using UnityEngine;

namespace World.Controller {
    public class CollectableController : MonoBehaviour {
        [SerializeField] private float ShrinkSpeed;
        [SerializeField] private ScriptableSignal CollectablesPickUp;
        
        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Player")) {
                CollectablesPickUp.Fire();
                StartCoroutine(DestructionRoutine());
            }
        }

        private IEnumerator DestructionRoutine() {
            while (transform.localScale.x > 0) {
                transform.localScale -= Vector3.one * ShrinkSpeed * Time.deltaTime;
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}