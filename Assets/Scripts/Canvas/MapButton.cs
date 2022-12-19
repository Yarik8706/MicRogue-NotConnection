using MainScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Canvas
{
    public class MapButton : MonoBehaviour
    {
        [Header("Camera")]
        public Camera mapCamera;
        public Camera mainCamera;
        
        [Header("Canvas")]
        public GameObject canvas;
        public GameObject mapCanvas;
        
        [Header("Components")]
        public Image imageRenderer;
        public Sprite enebleMap;
        public Sprite activeMap;

        public void ChangeMapActive()
        {
            if(GameManager.instance.moveToTheNextRoom) return;
            ChangeMap(mapCanvas.activeSelf);
        }
        
        public void EnableMap()
        {
            if(GameManager.instance.moveToTheNextRoom) return;
            ChangeMap(false);
        }
        
        private void ChangeMap(bool isMainMap)
        {
            mapCamera.enabled = !isMainMap;
            mainCamera.enabled = isMainMap;
            canvas.SetActive(isMainMap);
            mapCanvas.SetActive(!isMainMap);
            imageRenderer.sprite = isMainMap ? activeMap : enebleMap;
        }
        
        public void DisableMap()
        {
            ChangeMap(true);
        }
    }
}
