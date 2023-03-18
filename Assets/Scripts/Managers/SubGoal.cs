using DataTypes;
using DefaultNamespace.Signals;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Managers {
    public class SubGoal : MonoBehaviour {
        [SerializeField] private TMP_Text CurrentCounter;
        [SerializeField] private TMP_Text DesiredCounter;
        [SerializeField] private Image CollectableImage;

        [SerializeField] private CollectablesSignal CollectionPickupSignal;

        private CollectablesService collectablesService => ServiceLibrary.GetService<CollectablesService>();

        private CollectableTypesEnum _type;
        
        public void Setup(CollectableTypesEnum collectableType, int desiredAmount) {
            _type = collectableType;
            CollectionPickupSignal.RegisterResponse(OnCollectablePickup);
            
            var collectableData = collectablesService.GetCollectableInfo(_type);
            
            CollectableImage.sprite = collectableData.sprite;
            CollectableImage.color = collectableData.spriteColor;
            
            DesiredCounter.text = desiredAmount.ToString();
            UpdateCurrentCounter();
        }

        private void OnCollectablePickup(CollectableTypesEnum type) {
            if (type == _type) {
                UpdateCurrentCounter();
            }
        }

        private void UpdateCurrentCounter() {
            CurrentCounter.text = collectablesService.GetCollectedAmount(_type).ToString();
        }

        private void OnDestroy() {
            CollectionPickupSignal.UnregisterResponse(OnCollectablePickup);
        }
    }
}