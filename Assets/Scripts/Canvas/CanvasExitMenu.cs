using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Canvas
{
    public class CanvasExitMenu : MonoBehaviour
    {
        private bool _isMove;

        private IEnumerator Move(Vector2 targetPosition)
        {
            if (_isMove)
            {
                yield break;
            }
            _isMove = true;
            transform.position = targetPosition;
            yield return null;
            _isMove = false;
        }

        private void OnEnable()
        {
            StartCoroutine(Move(new Vector3(Screen.width/2, Screen.height/2)));
        }

        public void CloseDialog()
        {
            StartCoroutine(CloseDialogCourotine());
        }

        private IEnumerator CloseDialogCourotine()
        {
            yield return Move(new Vector2(Screen.width / 2, Screen.height * 1.2f));
            gameObject.SetActive(false);
        }
        
        public void ExitOnMainMenu()
        {
            SceneManager.LoadScene(0);
        }
    }
}
