using UnityEngine;

namespace DefaultNamespace.Interfaces.Character {
    public interface ICharacter {
        public CharacterController CharacterController { get; }
        public Transform Transform { get; }
        public Animator Animator { get; }

        public float forwardVelocity { get; }
        
        
        public Transform currentSpeedArrow { get; }
        public Transform normalArrow { get; }
        public Transform forwardArrow { get; }
    }
}