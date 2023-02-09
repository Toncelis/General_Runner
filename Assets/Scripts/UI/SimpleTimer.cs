using UnityEngine;

public class SimpleTimer : MonoBehaviour {
    [SerializeField] private ShadowedText Text;
    
    private float _seconds = 0;
    private int _minutes = 0;
    
    private void Update() {
        _seconds += Time.deltaTime;
        while (_seconds >= 60) {
            _minutes++;
            _seconds -= 60;
        }

        Text.text = $"{_minutes}:{_seconds:00.}";
    }
}
