using System;
using DG.Tweening;
using Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Managers {
    public class RoomGoal : MonoBehaviour{
        [SerializeField] private Image GoalImage;
        [SerializeField] private Image GoalProgressBar;

        [SerializeField] private Transform SubGoalsPanel;
        [SerializeField] private SubGoal SubGoalPrefab; 

        public void ChangeFilling(float value) {
            GoalProgressBar.DOFillAmount(value, 0.1f);
        }

        public void ChangePosition(float xValue) {
            var localPosition = GoalImage.rectTransform.localPosition;
            Vector3 position = new Vector3(xValue, localPosition.y, localPosition.z);
            transform.DOLocalMove(position, 1);
        }

        public void Setup (Vector3 rectPosition, RoomChangingRule changingRule) {
            transform.localPosition = rectPosition;
            GoalImage.sprite = changingRule.nextRoom.roomSprite;

            GoalProgressBar.fillAmount = 1f;
            transform.localScale = Vector3.zero;

            for (int i = 0; i < changingRule.requirements.Count; i++) {
                var requirement = changingRule.requirements[i];
                var subGoal = Instantiate(SubGoalPrefab, SubGoalsPanel);
                subGoal.transform.localPosition = subGoal.transform.localPosition.WithY(16 - i * 20);
                subGoal.Setup(requirement.collectableType, requirement.requiredAmount);
            }
            
            Show();
        }

        public void HideSubGoals() {
            SubGoalsPanel.DOLocalMoveY(SubGoalsPanel.localPosition.y + 100, 1f);
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
