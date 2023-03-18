using DataTypes;
using DefaultNamespace.World.View;
using Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DefaultNamespace.Managers.TrackObjects {
    public class OffsetManager : TrackObjectManager {
        [SerializeField] private bool UseDiscreteOffsetValues = true;
        [SerializeField, ShowIf(nameof(UseDiscreteOffsetValues))]
        private WeightedList<float> OffsetVariantsList;
        [SerializeField, HideIf(nameof(UseDiscreteOffsetValues))]
        private float OffsetRange;
        
        [PropertySpace(4)]
        [SerializeField] private Transform MainTransform; 

        public override void Setup(TileView tile, float positionViaLength) {
            float offset = UseDiscreteOffsetValues ? OffsetVariantsList.GetRandomObject() : Random.Range(-OffsetRange, OffsetRange);

            var(position, direction) = tile.GetPositionAndDirectionFromLength(positionViaLength);
            var offsetDirection = Quaternion.Euler(0,90,0) * direction.WithY(0);
            
            MainTransform.position = position + offsetDirection * offset;
            MainTransform.LookAt(direction);
        }
    }
}