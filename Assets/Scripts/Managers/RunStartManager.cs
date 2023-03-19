using System.Linq;
using DataTypes;
using DefaultNamespace.Signals;
using Services;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DefaultNamespace.Managers {
    public class RunStartManager : MonoBehaviour, ISourceOfServiceDependencies {
        [SerializeField] private StartSettings StartSettings;
        [SerializeField] private float VisibilityDistance;
        [SerializeField] private Transform RoadHolder;
        public StartSettings startSettings => StartSettings;
        public float visibilityDistance => VisibilityDistance;
        public Transform roadHolder => RoadHolder;

        [Space(4)]
        [SerializeField] private CollectableData[] AllCollectables;
        public CollectableData[] allCollectables => AllCollectables.ToArray();
        
        [Space(4)]
        [SerializeField, BoxGroup("Signals")] private CollectablesSignal CollectablePickup;
        [SerializeField, BoxGroup("Signals")] private CollectablesSignal CollectableLocked;
        [SerializeField, BoxGroup("Signals")] private CollectablesSignal CollectableUnlocked;
        public CollectablesSignal collectablePickup => CollectablePickup;
        public CollectablesSignal collectableLocked => CollectableLocked;
        public CollectablesSignal collectableUnlocked => CollectableUnlocked;
        
        private ServiceLibrary _serviceLibrary;
        
        public void OnEnable() {
            _serviceLibrary = ServiceLibrary.InitLibrary();

            ServiceLibrary.SetupService<WorldTilesService>(this);
            ServiceLibrary.SetupService<PositionService>(this);

            ServiceLibrary.SetupService<CollectablesService>(this);
        }

        public void OnDisable() {
            _serviceLibrary.ClearLibrary();
        }
    }
}