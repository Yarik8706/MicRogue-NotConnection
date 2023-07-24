using System;
using DG.Tweening;
using Enemies;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace Canvas
{
    public class TrainingControllerUI : MonoBehaviour
    {
        [SerializeField] private GameObject dialog;
        [SerializeField] private Transform dialogMovePosition;
        [SerializeField] private TMP_Text text;
        
        private Cause[] activeDialogTexts;
        private Action endDialogAction;
        private int activeTextIndex;

        public void StartDialog(Cause[] newDialogTexts, Action newEndAction)
        {
            dialog.transform.DOMove(
                dialogMovePosition.position, 
                0.5f).SetEase(Ease.InCubic);
            activeDialogTexts = newDialogTexts;
            endDialogAction = newEndAction;
            activeTextIndex = 0;
            text.text = activeDialogTexts[0].GetCause();
        }

        public void NextDialogText()
        {
            if (activeTextIndex + 1 == activeDialogTexts.Length)
            {
                dialog.transform.DOMove(
                    dialogMovePosition.position - Vector3.up * dialogMovePosition.position.y * 2, 
                    0.5f).SetEase(Ease.InCubic);
                endDialogAction?.Invoke();
                return;
            }
            activeTextIndex++;
            text.text = activeDialogTexts[activeTextIndex].GetCause();
        }
    }
}