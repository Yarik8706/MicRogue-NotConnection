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

        private IEnumerator Move(Vector3 target)
        {
            if (_isMove) yield break;
            _isMove = true;
            deathMessageObject.transform.DOMove(target, 0.7f)
                .SetEase(Ease.InCubic);
            yield return new WaitForSeconds(0.7f);
            _isMove = false;
        }

        public void ShowMessage(string message)
        {
            transform.position += Vector3.up * transform.position.y * 1.5f;
            deathMessageObject.SetActive(true);
            deathMessageText.text = message;
            _isActive = true;
            StartCoroutine(Move(transform.position - Vector3.up * transform.position.y * 1.5f));
        }

        private void CloseDialogAndReloadScene()
        {
            StartCoroutine(CloseDialogCoroutine());
        }

        private IEnumerator CloseDialogCoroutine()
        {
            yield return Move(transform.position + Vector3.up * transform.position.y * 1.5f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            deathMessageObject.SetActive(false);
        }
    }
}
