using System;
using DataTypes;
using Sirenix.OdinInspector;
using UnityEngine;
namespace World.Model {
    [Serializable]
    public class TrackObjectSettings {
        [SerializeField] private TrackObjectConfig ObjectConfig;
        [HorizontalGroup("Length"), LabelWidth(100)]
        [SerializeField] private bool OverrideLength;
        [HorizontalGroup("Length"), HideLabel]
        [SerializeField, ShowIf(nameof(OverrideLength))] private float Length;
        [HorizontalGroup("BlockingLength"), LabelWidth(100)]
        [SerializeField] private bool OverrideBlockingLength;
        [HorizontalGroup("BlockingLength"), HideLabel]
        [SerializeField, ShowIf(nameof(OverrideBlockingLength))] private float BlockingLength;
        [HorizontalGroup("Offset"), LabelWidth(100)]
        [SerializeField] private bool OverrideOffset;
        [HorizontalGroup("Offset"), HideLabel]
        [SerializeField, ShowIf(nameof(OverrideOffset))] private WeightedList<float> OffsetVariants;

        public TrackObjectConfig config => ObjectConfig;
        public float length => OverrideLength?Length:ObjectConfig.length;
        public float offset => OverrideOffset?OffsetVariants.GetRandomObject():ObjectConfig.offset;
        public float blockingLength => OverrideBlockingLength ? BlockingLength : config.blockingLength;
    }
}