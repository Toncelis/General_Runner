using TMPro;
using UnityEngine;

public class ShadowedText : MonoBehaviour {
    [SerializeField] private TMP_Text _text;
    [SerializeField] private TMP_Text _shadow;
    
    public string text {
        set {
            _text.text = value;
            _shadow.text = value;
        }
    }

    public Vector3 localScale {
        get => _shadow.rectTransform.localScale;
        set => _shadow.rectTransform.localScale = value;
    }
}
