using MainScripts;
using UnityEngine;

namespace Canvas
{
    public class SetCameraPlayerPosition : MonoBehaviour
    {
        public static SetCameraPlayerPosition instance;
        
        private void Start()
        {
            instance = this;
        }

        public void SetCameraPositionPlayerPosition()
        {
            var camera = Camera.main!;
            camera.transform.position = new Vector3(
                GameManager.player.transform.position.x,
                GameManager.player.transform.position.y,
                camera.transform.position.z
            );
        }
    }
}