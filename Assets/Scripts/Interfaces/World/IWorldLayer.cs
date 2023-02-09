using DefaultNamespace.Managers;
using UnityEngine;

namespace DefaultNamespace.Interfaces.World {
    public interface IWorldLayer {
        public WorldLayerManager GenerateLayer();
        public void RemoveLayer();
        
        public Camera GetLayerCamera();
        public RenderTexture GetViewTexture();
    }
}