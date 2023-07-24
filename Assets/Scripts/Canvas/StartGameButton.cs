using UnityEngine;
using UnityEngine.SceneManagement;

namespace Canvas
{
    public class StartGameButton : MonoBehaviour
    {
        public void StartGame()
        {
            SceneManager.LoadScene(1);
        }
    }
}