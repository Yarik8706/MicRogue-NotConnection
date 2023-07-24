using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Canvas
{
    public class DeathMesssageUI : MonoBehaviour
    {
        [SerializeField] private GameObject deathMessageObject;
        [SerializeField] private TMP_Text deathMessageText;
        
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

        private IEnumerator Move()
        {
            if (_isMove) yield break;
            _isMove = true;
            yield return new WaitForSeconds(0.7f);
            _isMove = false;
        }

        public void ShowMessage(string message)
        {
            deathMessageObject.SetActive(true);
            deathMessageText.text = message;
            _isActive = true;
            StartCoroutine(Move());
        }

        private void CloseDialogAndReloadScene()
        {
            StartCoroutine(CloseDialogCoroutine());
        }

        private IEnumerator CloseDialogCoroutine()
        {
            yield return Move();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            deathMessageObject.SetActive(false);
        }
    }
}
