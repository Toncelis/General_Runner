﻿using DefaultNamespace.EditorTools.Splines;
using DefaultNamespace.Managers;
using DefaultNamespace.World.View;
using Extensions;
using UnityEngine;

namespace Services {
    public class PositionService : Service {
        private TileView _currentTile;
        private TileView _previousTile;
        private TileView _nextTile;

        private int _tileIndex;
        private int _subcurveIndex;
        private int _segmentIndex;

        private Vector3 _segmentStart;
        private Vector3 _segmentEnd;

        private WorldTilesManager _tilesManager;
        public override void SetupService() {
            _tilesManager = GameObject.FindObjectOfType<WorldTilesManager>();
            _tilesManager.InitialGeneration();
            _previousTile = null;
            _currentTile = _tilesManager.GetTile(0);
            _nextTile = _tilesManager.GetTile(1);

            var characterManager = GameObject.FindObjectOfType<CharacterManager>();
            //characterManager.transform.position = Vector3.zero;
            _segmentIndex = 0;
            _subcurveIndex = 0;
            _tileIndex = 0;
            
            _segmentStart = _currentTile.transform.TransformPoint(_currentTile.Spline.Bezier(_subcurveIndex, (float)_segmentIndex / _currentTile.Density));
            _segmentEnd = _currentTile.transform.TransformPoint(_currentTile.Spline.Bezier(_subcurveIndex, (float)(_segmentIndex + 1) / _currentTile.Density));
            
            characterManager.StartMovement();
        }

        
        public void ProcessNewPosition(Vector3 position) {
            var flatPosition = position.XZProjection();
            var flatStart = _segmentStart.XZProjection();
            var flatEnd = _segmentEnd.XZProjection();

            var projection = Vector3.Project(flatPosition - flatStart, flatEnd - flatStart);
            if (projection.sqrMagnitude >= (flatEnd - flatStart).sqrMagnitude) {
                ProgressToNextSegment();
            }
        }
        
        private void ProgressToNextSegment() {
            _segmentIndex++;
            if (_segmentIndex >= _currentTile.Density) {
                _segmentIndex = 0;
                ProgressToNextSubcurve();
            }

            _segmentStart = _currentTile.transform.TransformPoint(_currentTile.Spline.Bezier(_subcurveIndex, (float)_segmentIndex / _currentTile.Density));
            _segmentEnd = _currentTile.transform.TransformPoint(_currentTile.Spline.Bezier(_subcurveIndex, (float)(_segmentIndex + 1) / _currentTile.Density));
        }

        private void ProgressToNextSubcurve() {
            _subcurveIndex++;
            if (_subcurveIndex < _currentTile.Spline.count-1) {
                return;
            }
            _subcurveIndex = 0;

            ProgressToNextTile();
        }

        private void ProgressToNextTile() {
            _previousTile = _currentTile;
            _currentTile = _nextTile;
            _tileIndex++;
            _nextTile = _tilesManager.GetTile(_tileIndex + 1);

            Debug.Log($"tile index : {_tileIndex}. LastTile index :{_tilesManager.LastTileIndex}");
            
            if (_tileIndex + 3 > _tilesManager.LastTileIndex) {
                _tilesManager.LoadNextTile();
            }
            _tilesManager.RemoveTile(_tileIndex - 2);
        }

        public Vector3 ForwardVector {
            get {
                Vector3 direction = _segmentEnd - _segmentStart;
                return direction;
            }
        }
    }
}