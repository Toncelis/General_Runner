using System;
using DG.Tweening;
using Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Managers {
    public class RoomGoal : MonoBehaviour{
        [SerializeField]
        private Image goalImage;
        [SerializeField]
        private Image goalProgressBar;

        [SerializeField] private Transform SubGoalsPanel;
        [SerializeField] private SubGoal SubGoalPrefab; 

        public void ChangeFilling(float value) {
            goalProgressBar.DOFillAmount(value, 0.1f);
            //transform.DOPunchScale(Vector3.one * 1.01f, 0.1f);
        }

        public void ChangePosition(float xValue) {
            Vector3 position = new Vector3(xValue, goalImage.rectTransform.localPosition.y, goalImage.rectTransform.localPosition.z);
            transform.DOLocalMove(position, 1);
        }

        public void Setup (Vector3 rectPosition, RoomChangingRule changingRule) {
            transform.localPosition = rectPosition;
            goalImage.sprite = changingRule.NextRoom.RoomSprite;

            goalProgressBar.fillAmount = 1f;
            transform.localScale = Vector3.zero;

            for (int i = 0; i < changingRule.Requirements.Count; i++) {
                var requirement = changingRule.Requirements[i];
                var subGoal = Instantiate(SubGoalPrefab, SubGoalsPanel);
                subGoal.transform.localPosition = subGoal.transform.localPosition.WithY(10 - i * 10);
                subGoal.Setup(requirement.collectable, requirement.amount, Color.blue);
            }
            
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
