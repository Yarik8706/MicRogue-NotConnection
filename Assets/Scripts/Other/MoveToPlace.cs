using MainScripts;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveToPlace : MonoBehaviour, IPointerClickHandler
{
    private Player _player;
    
    private void Start()
    {
        _player = GameManager.player;
    }
    
    protected virtual void Active()
    {
        StartCoroutine(_player.Move(transform.position));
        Destroy(gameObject);
    }
    
    public void OnPointerClick(PointerEventData @event)
    {
        Active();
    }
}

