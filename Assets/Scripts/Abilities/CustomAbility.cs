using System;
using System.Collections;
using MainScripts;
using UnityEngine;
using UnityEngine.UI;

public class CustomAbility : ScriptableObject
{
    [SerializeField] private bool isActiveAbility;
    [SerializeField] private Sprite buttonIcon;
    [SerializeField] private float updatePlayerTurnAfterUseSpellTime = .7f;
    
    public virtual void Initial(Image button)
    {
        if (!isActiveAbility)
        {
            button.gameObject.SetActive(false);
        }
        else
        {
            button.gameObject.SetActive(true);
            button.sprite = buttonIcon;
        }
    }

    public virtual void ActiveAbility()
    {
        
    }

    public virtual void DeleteAbility()
    {
        
    }

    protected void UpdatePlayerTurnAfterUseSpell()
    {
        CoroutineController.instance.StartCoroutine(
            UpdatePlayerTurnAfterUseSpellCoroutine(updatePlayerTurnAfterUseSpellTime));
    }

    protected static IEnumerator UpdatePlayerTurnAfterUseSpellCoroutine(float waitTime)
    {
        GameManager.player.DeleteAllMoveToPlaces();
        yield return new WaitForSeconds(waitTime);
        GameManager.player.Active();
    }
}