using System;
using DataTypes;
using UnityEngine;

namespace World.Model {
    [CreateAssetMenu(menuName = "Configs/Tile", order = 1, fileName = "Tile_NAME")]
    public class TileConfig : ScriptableObject {
        [SerializeField]
        private WeightedList<TileConfig> NextTileOptions;
        [SerializeField]
        private WeightedList<TrackObjectConfig> TrackObjectOptions;
        [SerializeField]
        private GameObject TilePrefab;

        [SerializeField]
        private float _initialBlankSpace = 5f;
        [SerializeField]
        private float _minBlankSpace = 2f;
        [SerializeField]
        private float _maxBlankSpace = 5f;

        [SerializeField] private TileConfig normalizingTile;

        public float InitialBlankSpace => _initialBlankSpace;
        public float MinBlankSpace => _minBlankSpace;
        public float MaxBlankSpace => _maxBlankSpace;
        public TileConfig NormalizingTile => normalizingTile;

        public TileConfig GetNextTile() {
            return NextTileOptions.GetRandomObject();
        }

        public bool GetNextTrackObject(Predicate<TrackObjectConfig> condition, out TrackObjectConfig objConfig) {
            return TrackObjectOptions.GetRandomObjectWithCondition(condition, out objConfig);
        }

        public GameObject prefab => TilePrefab;
    }
}