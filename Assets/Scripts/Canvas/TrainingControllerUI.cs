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
        [SerializeField] private TMP_Text text;
        
        private Cause[] activeDialogTexts;
        private Action endDialogAction;
        private int activeTextIndex;

        public void StartDialog(Cause[] newDialogTexts, Action newEndAction)
        {
            transform.position = new Vector3(transform.position.x, -Screen.height * 0.2f);
            dialog.transform.DOMove(
                transform.position + Vector3.up * Screen.height * 0.3f, 
                0.7f).SetEase(Ease.InCubic);
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
                    transform.position - Vector3.up * Screen.height * 0.6f, 
                    0.7f).SetEase(Ease.InCubic);
                endDialogAction?.Invoke();
                return;
            }
            activeTextIndex++;
            text.text = activeDialogTexts[activeTextIndex].GetCause();
        }
    }
}