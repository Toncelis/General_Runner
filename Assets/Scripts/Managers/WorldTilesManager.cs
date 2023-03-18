using System;
using System.Collections.Generic;
using DefaultNamespace.World.View;
using Extensions;
using UnityEngine;
using World.Model;

namespace DefaultNamespace.Managers {
    public class WorldTilesManager : MonoBehaviour {
        [SerializeField] private StartSettings StartSettings;
        [SerializeField] private float VisibilityDistance;

        private readonly Dictionary<int, TileView> _generatedTiles = new();
        private readonly Dictionary<int, Action> _onTileEnter = new();

        private int _lastTileIndex = -1;
        private RoomSettings _roomSettings;

        private int LastTileIndex => _lastTileIndex;
        private TileView lastTile => _generatedTiles[LastTileIndex];

        public TileView GetTile(int index) {
            if (_generatedTiles.ContainsKey(index)) {
                return _generatedTiles[index];
            }

            Debug.LogError($"trying to access tile with invalid index : {index}");
            return null;
        }
        
        public void EnterTile(int tileIndex) {
            if (_onTileEnter.ContainsKey(tileIndex)) {
                _onTileEnter[tileIndex]();
            }
            
            GenerateTilesToCoverVisibleDistance(tileIndex, VisibilityDistance);
        }
        
        public void InitialGeneration() {
            _roomSettings = StartSettings.startingRoom;
            GenerateTile(_roomSettings.startRoadTile, Vector3.zero, Vector3.forward);

            GenerateTilesToCoverVisibleDistance(0, VisibilityDistance);
        }

        private void GenerateTilesToCoverVisibleDistance(int currentTileIndex, float visibilityRange) {
            float coveredDistance = 0;
            do {
                currentTileIndex++;
                if (!_generatedTiles.ContainsKey(currentTileIndex)) {
                    LoadNextTile();
                }
                coveredDistance += (_generatedTiles[currentTileIndex].exitPointWorld - _generatedTiles[currentTileIndex - 1].exitPointWorld).magnitude;
            } while (coveredDistance < visibilityRange && !_generatedTiles[currentTileIndex].TileConfig.isVisionBreaker);
        }

        private void LoadNextTile(Action onTileEnter = null) {
            var newTileConfig = lastTile.TileConfig.GetNextTile();
            LoadTile(newTileConfig, onTileEnter);
        }

        public void NormalizeAndLoadTile(TileConfig newTile, Action onTileEnter = null) {
            var normalizingTile = lastTile.TileConfig.normalizingTile;
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
                    _onTileEnter.Remove(_lastTileIndex);
                }

                _onTileEnter.Add(_lastTileIndex, OnTileEnterWithClearing);
            }
        }
        
        private void GenerateTile(TileConfig tileConfig, Vector3 position, Vector3 flatDirection) {
            _lastTileIndex++;

            var tileObject = Instantiate(tileConfig.prefab, transform);
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
    }
}