using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DefaultNamespace.UI {
    public class StartUI : MonoBehaviour {
        public Toggle SkipTutorial;
        public Toggle CustomStart;
        public TMP_Dropdown StartingRoomDropdown;
        public Button StartButton;
        public Button ExitButton;
        [Space(10)]      
        public StartSettings StartSettings;
        public string MainScene;
        [Space(10)]
        public RoomSettings[] RoomsArray;

        private const string SKIP_TUTORIAL = "skipTutorial";
        private const string CUSTOM_START = "customStart";
        private const string CUSTOM_ROOM = "customRoom";
        
        
        private void OnEnable() {
            SkipTutorial.onValueChanged.AddListener(OnSkipTutorialToggle);
            CustomStart.onValueChanged.AddListener(OnCustomStartToggle);
            StartingRoomDropdown.onValueChanged.AddListener(OnRoomStartDropdown);

            StartingRoomDropdown.value = PlayerPrefs.GetInt(CUSTOM_ROOM);
            OnRoomStartDropdown(StartingRoomDropdown.value);
            CustomStart.isOn = PlayerPrefs.GetInt(CUSTOM_START) == 1;
            OnCustomStartToggle(CustomStart.isOn);
            SkipTutorial.isOn = PlayerPrefs.GetInt(SKIP_TUTORIAL) == 1;
            OnSkipTutorialToggle(SkipTutorial.isOn);
            
            StartButton.onClick.AddListener(OnStart);
            ExitButton.onClick.AddListener(OnExit);
        }
        
        private void OnStart() {
            SceneManager.LoadScene(MainScene, LoadSceneMode.Single);
        }

        private void OnExit() {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        private void OnSkipTutorialToggle(bool toggle) {
            CustomStart.gameObject.SetActive(toggle);
            PlayerPrefs.SetInt(SKIP_TUTORIAL, toggle ? 1 : 0);
            if (toggle) {
                OnCustomStartToggle(CustomStart.isOn);
            } else {
                StartingRoomDropdown.gameObject.SetActive(false);
                StartSettings.SetStartingRoom(RoomsArray[0]);
            }
        }
        
        private void OnCustomStartToggle(bool toggle) {
            StartingRoomDropdown.gameObject.SetActive(toggle);
            PlayerPrefs.SetInt(CUSTOM_START, toggle ? 1 : 0);
            if (toggle) {
                OnRoomStartDropdown(PlayerPrefs.GetInt(CUSTOM_ROOM));
            } else {
                StartSettings.SetStartingRoom(RoomsArray[1]);
            }
        }

        private void OnRoomStartDropdown(int roomIndex) {
            PlayerPrefs.SetInt(CUSTOM_ROOM, roomIndex);
            StartSettings.SetStartingRoom(RoomsArray[roomIndex]);
        }
    }
}