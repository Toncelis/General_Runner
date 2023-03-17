using DefaultNamespace.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Managers {
    public class SubGoal : MonoBehaviour {
        public TMP_Text CurrentCounter;
        public TMP_Text DesiredCounter;
        public Image CollectableImage;

        public CollectablesSignal CollectionPickupSignal;

        private CollectableTypes _type;
        private int _currentAmount;
        
        public void Setup(CollectableTypes collectableType, int desiredAmount, Color color) {
            _currentAmount = 0;
            _type = collectableType;
            
            DesiredCounter.text = desiredAmount.ToString();
            CurrentCounter.text = _currentAmount.ToString();

            CollectableImage.color = color;
            
            CollectionPickupSignal.RegisterResponse(OnCollectablePickup);
        }

        private void OnCollectablePickup(CollectableTypes type) {
            if (type == _type) {
                _currentAmount++;
                CurrentCounter.text = _currentAmount.ToString();
            }
        }

        private void OnDestroy() {
            CollectionPickupSignal.UnregisterResponse(OnCollectablePickup);
        }
    }
}