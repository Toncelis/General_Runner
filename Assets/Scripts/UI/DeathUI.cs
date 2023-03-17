using System;
using DefaultNamespace.Signals;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DefaultNamespace.UI {
    public class DeathUI : MonoBehaviour {
        public RectTransform DeathPanel;
        public Button ExitButton;
        public Button RestartButton;

        public ScriptableSignal OnDeathSignal;
        public string StartMenuSceneName;
        
        private void OnExitButton() {
            SceneManager.LoadScene(StartMenuSceneName, LoadSceneMode.Single);
        }

        private void OnRestartButton() {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }

        private void OnEnable() {
            OnDeathSignal.RegisterResponse(OnDeath);
        }

        private void OnDisable() {
            OnDeathSignal.UnregisterResponse(OnDeath);
        }

        private void OnDeath() {
            DeathPanel.gameObject.SetActive(true);
            DeathPanel.transform.localScale = Vector3.zero;

            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(2f);
            sequence.Append(DeathPanel.DOScale(Vector3.one, 0.4f));
            
            ExitButton.onClick.AddListener(OnExitButton);
            RestartButton.onClick.AddListener(OnRestartButton);
        }
    }
}