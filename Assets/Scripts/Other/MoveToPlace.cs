using MainScripts;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveToPlace : MonoBehaviour, IPointerClickHandler
{
    private Player _player;
    public bool isActive { get; set; } = true;
    
    private void Start()
    {
        _player = GameManager.player;
    }
    
    protected virtual void Active()
    {
        _player.StartMove(transform.position);
        Destroy(gameObject);
    }
    
    public void OnPointerClick(PointerEventData @event)
    {
        if(!isActive) return;
        Active();
    }
}

