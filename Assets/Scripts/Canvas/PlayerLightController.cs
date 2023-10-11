using MainScripts;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLightController : MonoBehaviour
{
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite inactiveSprite;
    [SerializeField] private Image image;

    private bool isLightActive = true;

    public void ChangeLightActive()
    {
        isLightActive = !isLightActive;
        image.sprite = isLightActive ? activeSprite : inactiveSprite;
        GameManager.player.ChangeCenterLightActive(isLightActive);
    }
}