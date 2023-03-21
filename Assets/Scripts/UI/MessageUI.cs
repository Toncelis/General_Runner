using System;
using DG.Tweening;
using Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageUI : MonoBehaviour {
    public Transform TotalMessagePanel;

    public Transform MovingPart;
    public Transform MovingMask;
    public Transform StationaryPart;
    public Image SpeakerIcon;

    public TMP_Text MessageText;

    public float SymbolsPerSecond;
    public float TimeToOpenIcon;

    public RectMask2D TextMask;

    private Vector3 _maskOffset;
    private Vector3 _stationaryOffset;

    private Vector3 _movingPartStartPosition;
    private Vector3 _totalPanelStartPosition;

    private string _currentMessage;
    private int _symbolsToDisplay;

    private bool _textIsShown;
    private bool _messageBoardIsShown;

    private Tween _textTween = null;

    private bool _isShowingMessage;
    public bool isShowingMessage => _isShowingMessage;

    public void DisplayMessage(string text, Sprite speaker) {
        var fullSequence = DOTween.Sequence();
        if (!_messageBoardIsShown) {
            fullSequence.Append(ShowMessageBoard().OnComplete(() => _isShowingMessage=true));
        }

        fullSequence.Append(OpenIcon(speaker));
        fullSequence.Append(ShowMessage(text));

        fullSequence.AppendInterval(2f).OnComplete(() => _isShowingMessage=false);
        fullSequence.Append(CloseIcon());
        fullSequence.Join(HideMessageBoard());
    }
    
    private Tween ShowMessageBoard() {
        return TotalMessagePanel.DOMoveY(120, 1).OnUpdate(UpdatePositions).OnComplete(() => _messageBoardIsShown = true);
    }

    private Tween HideMessageBoard() {
        return TotalMessagePanel.DOMoveY(_totalPanelStartPosition.y, 2).OnUpdate(UpdatePositions).OnStart(() => _messageBoardIsShown = false);
    }

    private Tween ShowMessage(string message) {
        if (_textTween != null) {
            _textTween.Kill();
        }
        
        var sequence = DOTween.Sequence();
        if (_currentMessage != message) {
            if (_textIsShown) {
                sequence.Append(
                    HideMessage().OnComplete(() => SetText(message))
                );
            } else {
                SetText(message);
            }
        }
        return _textTween = DOTween.To(() => _symbolsToDisplay, x => _symbolsToDisplay = x, _currentMessage.Length + 4, _currentMessage.Length / SymbolsPerSecond)
            .OnUpdate(UpdateText)
            .OnComplete(()=>_textTween = null)
            .OnKill(()=>_textTween = null);
    }

    private Tween HideMessage() {
        return DOTween.To(() => TextMask.padding.z, x => TextMask.padding = TextMask.padding.WithW(0), 0, 0.5f).OnComplete(ResetText);
    }

    private Tween OpenIcon(Sprite icon) {
        MovingPart.DOKill();
        if (icon != SpeakerIcon.sprite) {
            Sequence sequence = DOTween.Sequence()
                .Append(CloseIcon().OnComplete(() => SwapIcon(icon)).OnUpdate(UpdatePositions))
                .Append(MovingPart.DOMoveX(_movingPartStartPosition.x - 100, TimeToOpenIcon).OnUpdate(UpdatePositions));
            return sequence;
        } else {
            return MovingPart.DOMoveX(_movingPartStartPosition.x - 100, TimeToOpenIcon).OnUpdate(UpdatePositions);
        }
    }

    private void SwapIcon(Sprite icon) {
        SpeakerIcon.sprite = icon;
    }

    private Tween CloseIcon() {
        return MovingPart.DOMoveX(_movingPartStartPosition.x, TimeToOpenIcon).OnUpdate(UpdatePositions);
    }

    private void SetText(string text) {
        _currentMessage = text;
        _symbolsToDisplay = -1;
        UpdateText();
    }

    private void ResetText() {
        TextMask.padding = TextMask.padding.WithW(110);
        SetText("");
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
}