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
        private bool WithVariations;

        private bool useVariations => !complex && WithVariations;
        [SerializeField, ShowIf(nameof(useVariations))]
        private WeightedList<GameObject> Prefabs;

        private bool usePrefab => !Complex && !WithVariations;
        [SerializeField, ShowIf(nameof(usePrefab))]
        private GameObject Prefab;
        public GameObject prefab => useVariations?Prefabs.GetRandomObject():Prefab;

        [SerializeField, ShowIf(nameof(Complex)), ListDrawerSettings(AddCopiesLastElement = true)]
        private TrackObjectSettings[] ObjectParts;
        public TrackObjectSettings[] objectParts => ObjectParts;

        [SerializeField] private bool UseBlockingDistance = false;
        [SerializeField, ShowIf(nameof(UseBlockingDistance))]
        private float BlockingLength;
        
        [SerializeField, HideIf(nameof(Complex))]
        private float Length;
        public float length => Complex ? ObjectParts.Sum(part => part.length) : Length;

        public float blockingLength => UseBlockingDistance 
            ? BlockingLength 
            : Complex 
                ? ObjectParts.Sum(part => part.blockingLength) 
                : Length;
        
        [SerializeField] private bool UseDiscreteOffsetValues = false;
        [SerializeField, ShowIf(nameof(UseDiscreteOffsetValues))]
        private WeightedList<float> OffsetVariantsList;
        [SerializeField, HideIf(nameof(UseDiscreteOffsetValues))]
        private float OffsetRange = 0;
        
        public float offset => UseDiscreteOffsetValues ? OffsetVariantsList.GetRandomObject() : Random.Range(-OffsetRange, OffsetRange);
    }
}