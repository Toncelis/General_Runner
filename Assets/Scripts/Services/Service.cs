using DataTypes;
using DefaultNamespace.Signals;
using UnityEngine;

namespace Services {
    public abstract class Service {
        public abstract void SetupService (ISourceOfServiceDependencies source);

        public virtual void CloseService() {}
    }

    public interface ISourceOfServiceDependencies {
        
        public StartSettings startSettings { get; }
        public float visibilityDistance { get; }
        public Transform roadHolder { get; }
        
        public CollectablesSignal collectablePickup { get; }
        public CollectablesSignal collectableLocked { get; }
        public CollectablesSignal collectableUnlocked { get; }
        
        public CollectableData[] allCollectables { get; }
        
        public MessageUI messageUI { get; }
    }
}