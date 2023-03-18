using UnityEngine;

namespace DefaultNamespace.Interfaces.Character {
    public interface ICharacter {
        public CharacterController characterController { get; }
        public Transform characterTransform { get; }
        public Animator animator { get; }

        public float forwardVelocity { get; }
        
        /*
        public Transform currentSpeedArrow { get; }
        public Transform normalArrow { get; }
        public Transform forwardArrow { get; }
        */
    }
}