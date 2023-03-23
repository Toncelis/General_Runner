using System;
using System.Collections.Generic;
using DefaultNamespace.World.View;
using Extensions;
using Services;
using UnityEngine;
using World.Model;
using Object = UnityEngine.Object;

namespace DefaultNamespace.Services {
    public class WorldTilesService : Service {
        private StartSettings _startSettings;
        private float _visibilityDistance;
        private Transform _roadHolder;

        private readonly Dictionary<int, TileView> _generatedTiles = new();
        private readonly Dictionary<int, Action> _onTileEnter = new();

        private int _lastTileIndex = -1;
        private RoomSettings _roomSettings;

        private int LastTileIndex => _lastTileIndex;
        private TileView lastTile => _generatedTiles[LastTileIndex];

        public override void SetupService (ISourceOfServiceDependencies source) {
            _startSettings = source.startSettings;
            _visibilityDistance = source.visibilityDistance;
            _roadHolder = source.roadHolder;
        }

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
            
            GenerateTilesToCoverVisibleDistance(tileIndex, _visibilityDistance);
        }
        
        public void InitialGeneration() {
            _roomSettings = _startSettings.startingRoom;
            GenerateTile(_roomSettings.startRoadTile, Vector3.zero, Vector3.forward);

            GenerateTilesToCoverVisibleDistance(0, _visibilityDistance);
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

            var tileObject = Object.Instantiate(tileConfig.prefab, _roadHolder);
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

        public void SetVisibilityDistance(float distance) {
            _visibilityDistance = distance;
        }
    }
}