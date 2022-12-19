using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Canvas
{
    public class DialogController : MonoBehaviour
    {
        public Transform dialogObject;
        public Text dialogText;
        public static DialogController instance;
        public string[] startTextDialog;
        public string[] secondTextDialog;
        public string[] centerTextDialog;
        public string[] endTextDialog;
        private string[] _centerText;
        private int _activeTextIndex;
        private Action<int> _nextTextEvent;

        private void Awake()
        {
            instance = this;
        }

        public void NextText()
        {
            _activeTextIndex++;
            _nextTextEvent.Invoke(_activeTextIndex);
            if (_activeTextIndex == _centerText.Length)
            {
                CloseDialog();
                return;
            }
            dialogText.text = _centerText[_activeTextIndex];
        }

        public void StartDialog(string[] text, Action<int> nextTextEvent)
        {
            _activeTextIndex = 0;
            _nextTextEvent = nextTextEvent;
            _centerText = text;
            dialogObject.DOMove(Vector3.right * transform.position.x 
                                + Vector3.up * Screen.height * 0.2f, 1f)
                .SetEase(Ease.Linear).OnComplete(() =>
                {
                    dialogText.text = text[0];
                });
        }

        public void CloseDialog()
        {
            dialogObject.DOMove(Vector3.right * transform.position.x 
                                + Vector3.up * -Screen.height * 0.3f, 1f)
                .SetEase(Ease.Linear).OnComplete(() =>
                {
                    _nextTextEvent.Invoke(-1);
                    dialogText.text = "";
                });
        }
    }
}