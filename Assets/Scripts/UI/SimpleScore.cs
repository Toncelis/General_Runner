using DefaultNamespace.Signals;
using UnityEngine;

public class SimpleScore : MonoBehaviour {
    [SerializeField] private ScriptableSignal CollectablesPickUp;
    [SerializeField] private ShadowedText ScoreText;
    [SerializeField] private Animator ScoreAnimation;
    
    private int _score = 0;
    private static readonly int _pop = Animator.StringToHash("Pop");


    public void OnEnable() {
        CollectablesPickUp.RegisterResponse(Increment);
    }

    private void OnDisable() {
        CollectablesPickUp.UnregisterResponse(Increment);
    }

    private void Increment() {
        _score++;
        ScoreText.text = _score.ToString();
        ScoreAnimation.SetTrigger(_pop);
    }
}
