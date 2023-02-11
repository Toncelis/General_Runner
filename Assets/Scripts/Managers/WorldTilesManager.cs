using System.Collections.Generic;
using DefaultNamespace.World.View;
using Extensions;
using UnityEngine;
using World.Model;

namespace DefaultNamespace.Managers {
    public class WorldTilesManager : MonoBehaviour {
        private readonly Dictionary<int, TileView> _generatedTiles = new();

        private TileView LastTile => _generatedTiles[LastTileIndex];
        private int _lastTileIndex = -1;
        public int LastTileIndex => _lastTileIndex;

        [SerializeField]
        private RoomSettings RoomSettings;
        
        public TileView GetTile(int index) {
            if (_generatedTiles.ContainsKey(index)) {
                return _generatedTiles[index];
            }

            Debug.LogError($"trying to access tile with invalid index : {index}");
            return null;
        }

        public void LoadNextTile() {
            var lastTile = LastTile;
            var newTileConfig = lastTile.TileConfig.GetNextTile();
            var exitDirection = lastTile.exitDirectionWorld.WithY(0);
            
            GenerateTile(newTileConfig, lastTile.exitPointWorld, exitDirection);
        }

        private void GenerateTile(TileConfig tileConfig, Vector3 position, Vector3 flatDirection) {
            _lastTileIndex++;
            
            var tileObject = Instantiate(tileConfig.prefab, parent : transform);
            tileObject.name = $"{tileConfig.name}_{_lastTileIndex}";
            tileObject.transform.position = position;
            tileObject.transform.forward = flatDirection;
            
            var tileView = tileObject.GetComponent<TileView>();
            tileView.Setup(tileConfig);
            
            _generatedTiles.Add(_lastTileIndex, tileObject.GetComponent<TileView>());
            
            
            Debug.Log("loading new tile", tileObject);
        }

        public void RemoveTile(int tileIndex) {
            if (!_generatedTiles.ContainsKey(tileIndex)) {
                return;
            }

            var tile = _generatedTiles[tileIndex];
            _generatedTiles.Remove(tileIndex);
            tile.Destroy();
        }

        public void InitialGeneration() {
            GenerateTile(RoomSettings.StartRoadTile, Vector3.zero, Vector3.forward);
            LoadNextTile();
            LoadNextTile();
        }
    }
}