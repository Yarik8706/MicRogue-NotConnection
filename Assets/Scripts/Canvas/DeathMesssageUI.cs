using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Canvas
{
    public class DeathMesssageUI : MonoBehaviour
    {
        public GameObject deathMessageObject;
        public TMP_Text deathMessageText;
        
        private bool _isMove;
        private bool _isActive;

        private void Update()
        {
            if (!_isActive || _isMove) return;
            if (Input.GetMouseButtonDown(0) 
                || Input.GetMouseButtonDown(1) 
                || Input.GetMouseButtonDown(2)) CloseDialogAndReloadScene();
            if (Input.touchCount > 0) CloseDialogAndReloadScene();
        }

        private IEnumerator Move(Vector3 targetPosition)
        {
            if (_isMove) yield break;
            _isMove = true;
            transform.DOMove(targetPosition, 1f).SetEase(Ease.Linear);
            yield return new WaitForSeconds(1f);
            _isMove = false;
        }

        public void ShowMessage(string message)
        {
            deathMessageObject.SetActive(true);
            deathMessageText.text = message;
            _isActive = true;
            StartCoroutine(Move(new Vector2(Screen.width/2, Screen.height/2)));
        }

        private void CloseDialogAndReloadScene()
        {
            StartCoroutine(CloseDialogCoroutine());
        }

        private IEnumerator CloseDialogCoroutine()
        {
            yield return Move(new Vector2(Screen.width/2, -200));
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            gameObject.SetActive(false);
        }
    }
}
