using DG.Tweening;
using TMPro;
using UnityEngine;

public class MessageUI : MonoBehaviour {
    public Transform TotalMessagePanel;

    public Transform MovingPart;
    public Transform MovingMask;
    public Transform StationaryPart;

    public TMP_Text MessageText;

    public float SymbolsPerSecond;

    private Vector3 _maskOffset;
    private Vector3 _stationaryOffset;

    private Vector3 _movingPartStartPosition;
    private Vector3 _totalPanelStartPosition;

    private string _currentMessage;
    private int _symbolsToDisplay;

    public void ShowMessageBoard() {
        TotalMessagePanel.DOMoveY(120, 1).OnUpdate(UpdatePositions);
    }

    public void HideMessageBoard() {
        TotalMessagePanel.DOMoveY(_totalPanelStartPosition.y, 2).OnUpdate(UpdatePositions);
    }

    public void ShowMessage(string message) {
        SetText(message);
        DOTween.To(() => _symbolsToDisplay, x => _symbolsToDisplay = x, _currentMessage.Length + 4, _currentMessage.Length / SymbolsPerSecond).OnUpdate(UpdateText);
    }
    
    private Tween OpenIcon(float duration) {
        return MovingPart.DOMoveX(_movingPartStartPosition.x - 100, duration).OnUpdate(UpdatePositions);
    }


    private Tween CloseIcon(float duration) {
        return MovingPart.DOMoveX(_movingPartStartPosition.x, duration).OnUpdate(UpdatePositions);
    }
    
    private void SetText(string text) {
        _currentMessage = text;
        _symbolsToDisplay = -1;
        UpdateText();
    }
    
    private void UpdateText() {
        var currentText = "";
        if (_symbolsToDisplay > 3) {
            currentText += _currentMessage.Substring(0, Mathf.Min(_symbolsToDisplay - 3, _currentMessage.Length));
        }
        if (_symbolsToDisplay > 2 && _symbolsToDisplay - 3 < _currentMessage.Length) {
            currentText += "<alpha=#FF>" + _currentMessage[_symbolsToDisplay - 3];
        }
        if (_symbolsToDisplay > 1 && _symbolsToDisplay - 2 < _currentMessage.Length) {
            currentText += "<alpha=#AA>" + _currentMessage[_symbolsToDisplay - 2];
        }
        if (_symbolsToDisplay > 0 && _symbolsToDisplay - 1 < _currentMessage.Length) {
            currentText += "<alpha=#66>" + _currentMessage[_symbolsToDisplay - 1];
        }
        if (_symbolsToDisplay >= 0 && _symbolsToDisplay < _currentMessage.Length) {
            currentText += "<alpha=#22>" + _currentMessage[_symbolsToDisplay];
        }
        if (_symbolsToDisplay + 1 < _currentMessage.Length) {
            currentText += "<alpha=#00>" + _currentMessage.Substring(_symbolsToDisplay + 1);
        }
        MessageText.text = currentText;
    }
    
    private void OnEnable() {
        _movingPartStartPosition = MovingPart.position;
        _totalPanelStartPosition = TotalMessagePanel.position;

        _maskOffset = MovingMask.position - _movingPartStartPosition;
        _stationaryOffset = StationaryPart.position - _totalPanelStartPosition;
        MessageText.text = "";
    }

    private void UpdatePositions() {
        MovingMask.position = MovingPart.position + _maskOffset;
        StationaryPart.position = TotalMessagePanel.position + _stationaryOffset;
    }

    private void TestMove() {
        SetText("hello! testing communication here!\nDo you copy?");
        Sequence sequence = DOTween.Sequence();
        sequence.Append(TotalMessagePanel.DOMoveY(120, 1));
        sequence.AppendInterval(1);
        sequence.Append(OpenIcon(1));
        sequence.AppendInterval(1);
        sequence.Join(DOTween.To(() => _symbolsToDisplay, x => _symbolsToDisplay = x, _currentMessage.Length + 4, _currentMessage.Length / SymbolsPerSecond).OnUpdate(UpdateText));
        sequence.Append(CloseIcon(1));
        sequence.Join(TotalMessagePanel.DOMoveY(_totalPanelStartPosition.y, 2));
    }
}