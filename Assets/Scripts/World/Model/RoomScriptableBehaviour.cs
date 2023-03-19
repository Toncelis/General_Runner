using UnityEngine;

namespace World.Model {
    public abstract class RoomScriptableBehaviour : ScriptableObject {
        public abstract void Play();
        public abstract void Stop();
    }
}