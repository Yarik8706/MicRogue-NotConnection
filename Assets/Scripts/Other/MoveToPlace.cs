using MainScripts;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveToPlace : MonoBehaviour, IPointerClickHandler
{
    public Player player;
    public bool isActive { get; set; } = true;

    protected virtual void Active()
    {
        player.StartMove(transform.position);
        Destroy(gameObject);
    }
    
    public void OnPointerClick(PointerEventData @event)
    {
        if(!isActive) return;
        Active();
    }
}

