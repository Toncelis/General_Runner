using DefaultNamespace.Signals;
using DefaultNamespace.World.View;
using Services;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DataTypes {
    [CreateAssetMenu(menuName = "Configs/CollectableData", order = 4, fileName = "Collectable_NAME")]
    public class CollectableData : ScriptableObject {
        [SerializeField] private CollectableView View;
        [SerializeField] private CollectableTypesEnum Type;

        [SerializeField] private Sprite Sprite;
        [SerializeField] private Color SpriteColor = Color.white;

        [FoldoutGroup("Signals")]
        [SerializeField] private CollectablesSignal LockedSignal;
        [FoldoutGroup("Signals")]
        [SerializeField] private CollectablesSignal UnlockedSignal;
        
        
        public CollectableView view => View;
        public CollectableTypesEnum type => Type;
        public Sprite sprite => Sprite;
        public Color spriteColor => SpriteColor;

        public CollectablesSignal lockedSignal => LockedSignal;
        public CollectablesSignal unlockedSignal => UnlockedSignal;

        private void OnEnable() {
            ServiceLibrary.GetService<CollectablesService>().RegisterCollectable(this);
        }
    }
}