using System;
using DataTypes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace World.Model {
    [CreateAssetMenu(menuName = "Configs/Tile", order = 1, fileName = "Tile_NAME")]
    public class TileConfig : ScriptableObject {
        [Title("Filling settings")]
        [SerializeField] private WeightedList<TrackObjectSettings> TrackObjectOptions;
        [PropertySpace(2)]
        [SerializeField]
        private GameObject TilePrefab;
        [PropertySpace(2)]
        [FoldoutGroup("Blanks")]
        [SerializeField] private float InitialBlankSpace = 5f;
        [FoldoutGroup("Blanks")]
        [SerializeField] private float MinBlankSpace = 2f;
        [FoldoutGroup("Blanks")]
        [SerializeField] private float MaxBlankSpace = 5f;
        
        [PropertySpace(4), Title("End settings")]
        [SerializeField] private WeightedList<TileConfig> NextTileOptions;
        [Space(2)]
        [SerializeField] private TileConfig NormalizingTile = null;
        
        [PropertySpace(4), Title("Extra")]
        [SerializeField] private bool IsVisionBreaker = false;
        
        
        
        public GameObject prefab => TilePrefab;

        public float initialBlankSpace => InitialBlankSpace;
        public float minBlankSpace => MinBlankSpace;
        public float maxBlankSpace => MaxBlankSpace;
        
        public TileConfig normalizingTile => NormalizingTile;
        public bool isVisionBreaker => IsVisionBreaker;

        public bool GetNextTrackObject(Predicate<TrackObjectSettings> condition, out TrackObjectSettings objSettings) {
            return TrackObjectOptions.GetRandomObjectWithCondition(condition, out objSettings);
        }

        public TileConfig GetNextTile() {
            return NextTileOptions.GetRandomObject();
        }
    }
}