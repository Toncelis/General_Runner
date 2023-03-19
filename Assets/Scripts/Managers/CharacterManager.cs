using System;
using DefaultNamespace.Interfaces.Character;
using DefaultNamespace.Run.Actions;
using DefaultNamespace.Signals;
using UnityEngine;

namespace DefaultNamespace.Managers {
    public class CharacterManager : MonoBehaviour, ICharacter {
        [SerializeField] private Animator CharacterAnimator;
        [SerializeField] private Transform CharacterTransform;
        [SerializeField] private CharacterController CharacterController;

        [SerializeField] private ScriptableSignal OnDeathSignal;
        
        [SerializeField] private CharacterAction StandardAction;

        public CharacterController characterController => CharacterController;
        public Transform characterTransform => CharacterTransform;
        public Animator animator => CharacterAnimator;

        public float forwardVelocity { get; }

        private void Start() {
            CharacterTransform.position = new Vector3(0, 0.1f, 1);
            StartMovement();
        }

        public void StartMovement() {
            StandardAction.RunAction(Vector3.zero, this, null);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit) {
            if (hit.transform.CompareTag("Danger")) {
                StandardAction.StopAction();
                CharacterAnimator.SetTrigger("Die");
                OnDeathSignal.Fire();
            }
        }

        /*
        public Transform _currentSpeedArrow;
        public Transform _normalArrow;
        public Transform _forwardArrow;
        
        public Transform currentSpeedArrow => _currentSpeedArrow;
        public Transform normalArrow => _normalArrow;
        public Transform forwardArrow => _forwardArrow;
        */
    }
}