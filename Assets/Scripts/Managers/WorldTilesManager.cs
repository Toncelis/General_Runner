using System;
using System.Collections.Generic;
using DefaultNamespace.World.View;
using Extensions;
using UnityEngine;
using World.Model;

namespace DefaultNamespace.Managers {
    public class WorldTilesManager : MonoBehaviour {
        private readonly Dictionary<int, TileView> _generatedTiles = new();
        private Dictionary<int, Action> OnTileEnter = new();

        private TileView lastTile => _generatedTiles[LastTileIndex];
        private int _lastTileIndex = -1;
        public int LastTileIndex => _lastTileIndex;

        private RoomSettings _roomSettings;
        
        public TileView GetTile(int index) {
            if (_generatedTiles.ContainsKey(index)) {
                return _generatedTiles[index];
            }

            Debug.LogError($"trying to access tile with invalid index : {index}");
            return null;
        }

        public void LoadNextTile(Action onTileEnter = null) {
            var newTileConfig = lastTile.TileConfig.GetNextTile();
            LoadTile(newTileConfig, onTileEnter);
        }

        public void NormalizeAndLoadTile(TileConfig newTile, Action onTileEnter = null) {
            var normalizingTile = lastTile.TileConfig.NormalizingTile;
            if (normalizingTile != null) {
                LoadTile(normalizingTile);
            }
            LoadTile(newTile, onTileEnter);
        }

        private void LoadTile(TileConfig tile, Action onTileEnter = null) {
            var exitDirection = lastTile.exitDirectionWorld.WithY(0);
            GenerateTile(tile, lastTile.exitPointWorld, exitDirection);
            if (onTileEnter != null) {
                void OnTileEnterWithClearing() {
                    onTileEnter();
                    OnTileEnter.Remove(_lastTileIndex);
                }

                OnTileEnter.Add(_lastTileIndex, OnTileEnterWithClearing);
            }
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
        }

        public void RemoveTile(int tileIndex) {
            if (!_generatedTiles.ContainsKey(tileIndex)) {
                return;
            }

            var tile = _generatedTiles[tileIndex];
            _generatedTiles.Remove(tileIndex);
            tile.Destroy();
        }

        public void EnterTile(int tileIndex) {
            if (OnTileEnter.ContainsKey(tileIndex)) {
                OnTileEnter[tileIndex]();
            }
        }

        public StartSettings startSettings;

        public void InitialGeneration() {
            _roomSettings = startSettings.StartingRoom;
            GenerateTile(_roomSettings.StartRoadTile, Vector3.zero, Vector3.forward);
            LoadNextTile();
            LoadNextTile();
        }
    }
}