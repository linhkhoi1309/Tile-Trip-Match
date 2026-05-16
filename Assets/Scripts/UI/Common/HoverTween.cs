using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class HoverTween : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("Scale to apply on hover")] [SerializeField] private float hoverScale = 1.08f;
    [Tooltip("Duration of the hover tween")] [SerializeField] private float hoverDuration = 0.12f;
    [Tooltip("Easing for the hover tween")] [SerializeField] private Ease hoverEase = Ease.OutBack;

    private Tween hoverTween;

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverTween?.Kill();
        hoverTween = transform.DOScale(hoverScale, hoverDuration).SetEase(hoverEase).SetLink(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverTween?.Kill();
        hoverTween = transform.DOScale(1f, hoverDuration).SetEase(hoverEase).SetLink(gameObject);
    }

    private void OnDisable()
    {
        hoverTween?.Kill();
    }
}
