using DefaultNamespace.Services;
using UnityEngine;

namespace DefaultNamespace.Managers {
    public class ServicesManager : MonoBehaviour {
        private static WorldLayerConfigurator _WorldLayerConfigurator = null;
        public static WorldLayerConfigurator WorldLayerConfigurator => _WorldLayerConfigurator;
        
        private void Start() {
            _WorldLayerConfigurator = new WorldLayerConfigurator();
            
            WorldLayerConfigurator.GenerateLayer(WorldLayerTypes.Abstract);   
        }
    }
}