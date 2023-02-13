using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace World.Model {
    [CreateAssetMenu(menuName = "Configs/TrackObject", order = 2, fileName = "TrackObject_NAME")]
    public class TrackObjectConfig : ScriptableObject {
        [SerializeField]
        private bool complex = false;
        public bool Complex => complex;
        
        [SerializeField, HideIf(nameof(complex))]
        private GameObject prefab;
        public GameObject Prefab => prefab;

        [SerializeField, ShowIf(nameof(complex))]
        private TrackObjectConfig[] objectParts;
        public TrackObjectConfig[] ObjectParts => objectParts;

        [SerializeField, HideIf(nameof(complex))]
        private float length;
        public float Length => complex ? objectParts.Sum(part => part.Length) : length;
    }
}