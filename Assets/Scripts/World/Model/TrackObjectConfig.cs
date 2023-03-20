using System.Linq;
using DataTypes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace World.Model {
    [CreateAssetMenu(menuName = "Configs/TrackObject", order = 2, fileName = "TrackObject_NAME")]
    public class TrackObjectConfig : ScriptableObject {
        [SerializeField]
        private bool Complex = false;
        public bool complex => Complex;
        
        [SerializeField, HideIf(nameof(Complex))]
        private GameObject Prefab;
        public GameObject prefab => Prefab;

        [SerializeField, ShowIf(nameof(Complex))]
        private TrackObjectConfig[] ObjectParts;
        public TrackObjectConfig[] objectParts => ObjectParts;

        [SerializeField] private bool CustomBlockingDistance = false;
        [SerializeField, ShowIf(nameof(CustomBlockingDistance))]
        private float BlockingLength;
        
        [SerializeField, HideIf(nameof(Complex))]
        private float Length;
        public float length => Complex ? ObjectParts.Sum(part => part.length) : Length;

        public float blockingLength => CustomBlockingDistance ? BlockingLength : Complex ? ObjectParts.Sum(part => part.blockingLength) : Length;
        
        [SerializeField] private bool UseDiscreteOffsetValues = false;
        [SerializeField, ShowIf(nameof(UseDiscreteOffsetValues))]
        private WeightedList<float> OffsetVariantsList;
        [SerializeField, HideIf(nameof(UseDiscreteOffsetValues))]
        private float OffsetRange = 0;
        
        public float offsetRange => UseDiscreteOffsetValues ? OffsetVariantsList.GetRandomObject() : Random.Range(-OffsetRange, OffsetRange);
    }
}