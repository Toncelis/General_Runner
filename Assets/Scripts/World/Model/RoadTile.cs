using DataTypes;
using UnityEngine;

namespace World.Model {
    [CreateAssetMenu(menuName = "Configs/RoadTile", fileName = "RoadTile")]
    public class RoadTile : ScriptableObject {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private WeightedList<RoadTile> _nextTiles;
        [SerializeField] private WeightedList<TrackObject> _trackObjects;
        [SerializeField] private WeightedList<LandscapeObject> _surroundingObjects;
        
        [SerializeField] private Vector2 _totalShift;
        [SerializeField] private Vector2 _forwardVectorAtTheEnd;
        [SerializeField] private GeometryRule _geometryRule;

        public GameObject prefab => _prefab;
        
        public RoadTile GetNextRoadTile() {
            return _nextTiles.GetRandomObject();
        }
        public TrackObject GetTrackObject() {
            return _trackObjects.GetRandomObject();
        }
        public LandscapeObject GetSurroundingObject() {
            return _surroundingObjects.GetRandomObject();
        }

        public Vector2 totalShift => _totalShift;
        public Vector2 forwardVectorAtTheEnd => _forwardVectorAtTheEnd;
    }
}