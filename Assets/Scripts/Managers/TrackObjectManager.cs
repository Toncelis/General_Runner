using DefaultNamespace.World.View;
using UnityEngine;

namespace DefaultNamespace.Managers {
    public abstract class TrackObjectManager : MonoBehaviour {
        public abstract void Setup(TileView tile, float positionViaLength);
    }
}