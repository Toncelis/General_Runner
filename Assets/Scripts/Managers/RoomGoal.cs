using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Managers {
    public class RoomGoal : MonoBehaviour{
        [SerializeField]
        private Image goalImage;
        [SerializeField]
        private Image goalProgressBar;

        public void ChangeFilling(float value) {
            goalProgressBar.DOFillAmount(value, 0.1f);
                
            goalImage.rectTransform.DOPunchScale(Vector3.one * 1.1f, 0.2f);
            goalProgressBar.rectTransform.DOPunchScale(Vector3.one * 1.1f, 0.2f);
        }

        public void ChangePosition(float xValue) {
            Vector3 position = new Vector3(xValue, goalImage.rectTransform.localPosition.y, goalImage.rectTransform.localPosition.z);
            goalImage.rectTransform.DOLocalMove(position, 1);
        }

        public void Setup (Vector3 rectPosition, Sprite sprite) {
            goalImage.rectTransform.localPosition = rectPosition;
            goalImage.sprite = sprite;
            goalImage.color = goalImage.color.WithAlpha(0);

            goalProgressBar.fillAmount = 1f;
            goalProgressBar.color = goalProgressBar.color.WithAlpha(0);

            Show();
        }

        public void Hide(Action onComplete = null) {
            goalImage.DOFade(0, 1).OnComplete(new TweenCallback(onComplete));
            goalProgressBar.DOFade(0, 1);
        }

        public void Show() {
            goalImage.DOFade(1, 1);
            goalProgressBar.DOFade(1, 1);
        }
    }
}
