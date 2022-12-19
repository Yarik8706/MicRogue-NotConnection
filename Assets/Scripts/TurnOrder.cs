using System.Collections;
using Canvas;
using DG.Tweening;
using UnityEngine;

public class TurnOrder : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private CanvasTurnOrder _canvasTurnOrder;
    
    // Start is called before the first frame update
    private void Start()
    {
        _canvasTurnOrder = FindObjectOfType<CanvasTurnOrder>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.color = new Color(255, 255, 255, 0);
        StartCoroutine(AllLifeActivities());
    }

    private IEnumerator AllLifeActivities()
    {
        var fadeColor = new Color(255, 255, 255, 0);
        var normalColor = new Color(255, 255, 255, 1);
        _spriteRenderer.DOColor(normalColor, 1f).SetEase(Ease.OutCubic);
        yield return new WaitUntil(() => _canvasTurnOrder.isEnded);
        _spriteRenderer.DOColor(fadeColor, 1f).SetEase(Ease.OutCubic);
    }
}
