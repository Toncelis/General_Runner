using System.Collections.Generic;
using DefaultNamespace.Interfaces.World;
using DefaultNamespace.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace.Services {
    public class WorldLayerConfigurator {
        private Dictionary<WorldLayerTypes, IWorldLayer> _generatedLayers = new();
        public void GenerateLayer(WorldLayerTypes layerType) {
            if (_generatedLayers.ContainsKey(layerType)) {
                return;
            }

            var activeScene = SceneManager.GetActiveScene();
            var newLayerScene = SceneManager.CreateScene($"{layerType}_Scene");
            SceneManager.SetActiveScene(newLayerScene);
            
            var layerManagerHolder = new GameObject($"{layerType}_LevelManagerHolder");
            layerManagerHolder.AddComponent<WorldLayerManager>();

            SceneManager.SetActiveScene(activeScene);
        }

        public void RemoveLayer(WorldLayerTypes layerType) {
            
        }
    }

    public enum WorldLayerTypes {
        Abstract,
        Medieval,
    }
}