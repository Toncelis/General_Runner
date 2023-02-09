using System;
using UnityEngine;

namespace DefaultNamespace.Interfaces.Character {
    public interface ICharacterAction {
        public void RunAction(Vector3 currentSpeed, ICharacter character, Action<Vector3> onEnd);

        public Vector3 StopAction();
    }
}