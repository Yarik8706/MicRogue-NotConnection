using System;
using UnityEngine;

namespace Canvas
{
    public class CameraSwipeControl : MonoBehaviour
    {
        private Vector2 touchStartPos;
        private Vector2 touchEndPos;
        private Vector2 swipeDelta;

        public static float sensitivity = 2.0f;

        private void Update()
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    touchStartPos = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    touchEndPos = touch.position;
                    swipeDelta = (touchEndPos - touchStartPos) 
                                 * sensitivity / 500;

                    // Применяем перемещение камеры
                    Vector3 newPos = transform.position - new Vector3(swipeDelta.x, swipeDelta.y, 0);
                    transform.position = newPos;

                    // Обновляем начальную позицию
                    touchStartPos = touch.position;
                }
            }
            else if (Input.GetMouseButton(0))
            {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                // Применяем перемещение камеры с помощью мыши
                Vector3 newPos = transform.position - new Vector3(mouseX, mouseY, 0) * sensitivity;;
                transform.position = newPos;
            }
        }
    }
}