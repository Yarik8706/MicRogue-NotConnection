using System;
using UnityEngine;
using UnityEngine.UI;

public class CustomAbility : ScriptableObject
{
    [SerializeField] private bool isActiveAbility;
    [SerializeField] private Sprite buttonIcon;
    
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
}