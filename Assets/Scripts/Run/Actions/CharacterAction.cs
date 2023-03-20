using System;
using System.Collections;
using DefaultNamespace.Interfaces.Character;
using DefaultNamespace.Managers;
using UnityEngine;

namespace DefaultNamespace.Run.Actions {
    public abstract class CharacterAction : ScriptableObject, ICharacterAction{
        protected ICharacter character;
        protected Vector3 currentSpeed;
        protected Vector3 lastFramePosition;

        protected Coroutine coroutine;
        
        public void RunAction(Vector3 currentSpeed, ICharacter character, Action<Vector3> onEnd) {
            this.character = character;
            this.currentSpeed = currentSpeed;
            lastFramePosition = character.characterTransform.position;

            coroutine = CoroutineManager.Instance.StartRoutine(ActionRoutine());
        }

        public Vector3 StopAction() {
            CoroutineManager.Instance.StopRoutine(coroutine);
            return currentSpeed;
        }

        protected abstract IEnumerator ActionRoutine();
    }
}