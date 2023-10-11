using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Canvas
{
    public class DeathMessageUI : MonoBehaviour
    {
        [SerializeField] private GameObject deathMessageObject;
        [SerializeField] private TMP_Text deathMessageText;
        
        private bool _isMove;

        public static DeathMessageUI deathMessageUI;

        private void Start()
        {
            deathMessageUI = this;
        }

        private IEnumerator Move(Vector3 target)
        {
            if (_isMove) yield break;
            _isMove = true;
            deathMessageObject.transform.DOMove(target, 1.2f).SetLink(gameObject)
                .SetEase(Ease.InCubic);
            yield return new WaitForSeconds(1.2f);
            _isMove = false;
        }

        public void ShowMessage(string message)
        {
            deathMessageObject.transform.position = new Vector3(Screen.width/2, Screen.height/2) + Vector3.up * Screen.height;
            deathMessageObject.SetActive(true);
            deathMessageText.text = message;
            StartCoroutine(Move(new Vector3(Screen.width/2, Screen.height/2)));
        }

        public void CloseDialogAndReloadScene()
        {
            StartCoroutine(CloseDialogCoroutine());
        }

        private IEnumerator CloseDialogCoroutine()
        {
            yield return Move(new Vector3(Screen.width/2, -Screen.height*0.5f));
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            deathMessageObject.SetActive(false);
        }
    }
}
