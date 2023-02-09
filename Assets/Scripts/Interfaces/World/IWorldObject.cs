using UnityEngine;

namespace DefaultNamespace.Interfaces.World {
    public interface IWorldObject {
        public Vector2 size { get; }
        public void Place(Vector3 placement);
    }
}