using System.Collections;
using PlayersScripts;
using UnityEngine;
using UnityEngine.UI;

public class CustomAbility : ScriptableObject
{
    [SerializeField] private bool isActiveAbility;
    [SerializeField] private Sprite buttonIcon;
    [SerializeField] private float updatePlayerTurnAfterUseSpellTime = .7f;
    
    public void InitialActiveButton(Image button)
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

    public virtual void InitialAbility(Player player)
    {
        
    }

    public virtual void ActiveAbility(Player player)
    {
        
    }

    public virtual void DeleteAbility(Player player)
    {
        
    }

    protected void UpdatePlayerTurnAfterUseSpell(Player player)
    {
        CoroutineController.instance.StartCoroutine(
            UpdatePlayerTurnAfterUseSpellCoroutine(player, updatePlayerTurnAfterUseSpellTime));
    }

    protected static IEnumerator UpdatePlayerTurnAfterUseSpellCoroutine(Player player, float waitTime)
    {
        player.DeleteAllMoveToPlaces();
        yield return new WaitForSeconds(waitTime);
        player.Active();
    }
}