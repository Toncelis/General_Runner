using UnityEngine;

namespace World.RoomBehaviour {
    public abstract class RoomScriptableBehaviour : ScriptableObject {
        public abstract void Play();
        public abstract void Stop();
    }
}