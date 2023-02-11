using System;
using DataTypes;
using UnityEngine;

namespace World.Model {
    [CreateAssetMenu(menuName = "Configs/Tile", order = 1, fileName = "Tile_NAME")]
    public class TileConfig : ScriptableObject {
        [SerializeField]
        private WeightedList<TileConfig> NextTileOptions;
        [SerializeField]
        private WeightedList<TrackObject> TrackObjectOptions;
        [SerializeField]
        private GameObject TilePrefab;

        public TileConfig GetNextTile() {
            return NextTileOptions.GetRandomObject();
        }

        public TrackObject GetNextTrackObject(Predicate<TrackObject> condition) {
            return TrackObjectOptions.GetRandomObjectWithCondition(condition);
        }
        
        public GameObject prefab => TilePrefab;
    }
}