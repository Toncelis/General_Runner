﻿using DefaultNamespace.Interfaces.Character;
using DefaultNamespace.Run.Actions;
using DefaultNamespace.Signals;
using UnityEngine;

namespace DefaultNamespace.Managers {
    public class CharacterManager : MonoBehaviour, ICharacter {
        [SerializeField]
        private Animator _characterAnimator;
        [SerializeField]
        private CharacterController _characterController;

        [SerializeField]
        private ScriptableSignal OnTileCenterReached;
        [SerializeField]
        private ScriptableSignal OnDeathSignal;
        
        public CharacterController CharacterController => _characterController;
        public Transform Transform => transform;
        public Animator Animator => _characterAnimator;

        public float forwardVelocity { get; }

        public CharacterAction StandardAction;

        public void StartMovement() {
            StandardAction.RunAction(Vector3.zero, this, null);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit) {
            if (hit.transform.CompareTag("Danger")) {
                StandardAction.StopAction();
                _characterAnimator.SetTrigger("Die");
                OnDeathSignal.Fire();
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Finish")) {
                OnTileCenterReached.Fire();
            }
        }

        
        public Transform _currentSpeedArrow;
        public Transform _normalArrow;
        public Transform _forwardArrow;
        
        public Transform currentSpeedArrow => _currentSpeedArrow;
        public Transform normalArrow => _normalArrow;
        public Transform forwardArrow => _forwardArrow;
    }
}