using System;
using DefaultNamespace.Signals;
using Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace World.Controller {
    public class WorldGeneration : MonoBehaviour {
        public SurroundingsGenerationSettings GenerationSettings;
        public float TrackLength;
        private Vector3 _roadEnd = Vector3.zero;
        private float _trackIdleSteps = 3f;

        public ScriptableSignal OnTileCenterReached;

        private GameObject oldRoadTile, currentRoadTile;
        private void GenerateRoad() {
            if (oldRoadTile != null) {
                Destroy(oldRoadTile);
            }
            oldRoadTile = currentRoadTile;
            currentRoadTile = new GameObject($"RoadTile {_roadEnd}");
            currentRoadTile.transform.position = _roadEnd;
            Instantiate(GenerationSettings.RoadTile, currentRoadTile.transform);
            GenerateSurroundings(TrackLength);
            GenerateCollectables(TrackLength);
            _roadEnd += Vector3.forward * TrackLength;
        }

        private void GenerateCollectables(float length) {
            float coveredLength = 3*_trackIdleSteps;
            while (coveredLength < length) {
                var obj = GenerationSettings.GetTrackObject();
                float lengthStep = obj.Size.y + _trackIdleSteps;
                
                Vector2 objPos = new Vector2(
                    Random.Range(-GenerationSettings.RoadWidth + obj.Size.x/2, GenerationSettings.RoadWidth - obj.Size.x/2),
                    coveredLength - 0.5f
                );
                coveredLength += lengthStep;
                Instantiate(obj.Prefab, objPos.ToXZByDirection(GenerationSettings.MainDirection) + _roadEnd, Quaternion.identity, currentRoadTile.transform);
            }
        }
        
        private void GenerateSurroundings(float length) {
            float coveredLength = 0f;
            while (coveredLength < length) {
                float lengthStep = Random.Range(GenerationSettings.MinSpaceFilling, GenerationSettings.MaxSpaceFilling);
                var obj = GenerationSettings.GetObject();
                lengthStep += obj.Size.y;
                coveredLength += lengthStep;
                
                Vector2 objPos = new Vector2(
                    Random.Range(GenerationSettings.RoadWidth + obj.Size.x / 2, GenerationSettings.MaxSurroundingWidth - obj.Size.x / 2),
                    coveredLength - obj.Size.y / 2
                );
                Instantiate(obj.Object, objPos.ToXZByDirection(GenerationSettings.MainDirection) + _roadEnd, Quaternion.identity, currentRoadTile.transform);
            }
            
            coveredLength = 0f;
            while (coveredLength < length) {
                float lengthStep = Random.Range(GenerationSettings.MinSpaceFilling, GenerationSettings.MaxSpaceFilling);
                var obj = GenerationSettings.GetObject();
                lengthStep += obj.Size.y;
                coveredLength += lengthStep;
                
                Vector2 objPos = new Vector2(
                    Random.Range(GenerationSettings.RoadWidth + obj.Size.x / 2, GenerationSettings.MaxSurroundingWidth - obj.Size.x / 2),
                    coveredLength - obj.Size.y / 2
                );
                objPos = new Vector2(-objPos.x, objPos.y);
                Instantiate(obj.Object, objPos.ToXZByDirection(GenerationSettings.MainDirection) + _roadEnd, Quaternion.identity, currentRoadTile.transform);
            }
        }

        private void OnEnable() {
            OnTileCenterReached.RegisterResponse(GenerateRoad);
            GenerateRoad();
        }

        private void OnDisable() {
            OnTileCenterReached.UnregisterResponse(GenerateRoad);
        }
    }
}