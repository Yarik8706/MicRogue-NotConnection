using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Canvas
{
    public class CanvasExitMenu : MonoBehaviour
    {
        private bool _isMove;

        private void Move(Vector2 targetPosition)
        {
            if (_isMove)
            {
                return;
            }
            _isMove = true;
            transform.position = targetPosition;
            _isMove = false;
        }

        private void OnEnable()
        {
            Move(new Vector3(Screen.width/2, Screen.height/2));
        }

        public void CloseDialog()
        {
            CloseDialogCourotine();
        }

        private void CloseDialogCourotine()
        {
            Move(new Vector2(Screen.width / 2, Screen.height * 1.2f));
            gameObject.SetActive(false);
        }
        
        public void ExitOnMainMenu()
        {
            SceneManager.LoadScene(0);
        }
    }
}
