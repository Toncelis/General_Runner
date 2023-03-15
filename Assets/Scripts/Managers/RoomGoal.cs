using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Managers {
    public class RoomGoal : MonoBehaviour{
        [SerializeField]
        private Image goalImage;
        [SerializeField]
        private Image goalProgressBar;

        public void ChangeFilling(float value) {
            Debug.Log($"changing filling of {this.name} to {value}", this);
            goalProgressBar.DOFillAmount(value, 0.1f);
                
            transform.DOPunchScale(Vector3.one * 1.01f, 0.1f);
        }

        public void ChangePosition(float xValue) {
            Vector3 position = new Vector3(xValue, goalImage.rectTransform.localPosition.y, goalImage.rectTransform.localPosition.z);
            transform.DOLocalMove(position, 1);
        }

        public void Setup (Vector3 rectPosition, Sprite sprite) {
            transform.localPosition = rectPosition;
            goalImage.sprite = sprite;

            goalProgressBar.fillAmount = 1f;
            transform.localScale = Vector3.zero;

            Show();
        }

        public void Hide(Action onComplete = null) {
            if (onComplete != null) {
                transform.DOScale(0, 1).OnComplete(new TweenCallback(onComplete));
            } else {
                transform.DOScale(0, 1);
            }
        }

        public void Show() {
            transform.DOScale(1, 1);
        }
    }
}
