using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ECToggle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool value;
    [SerializeField] private Image icon;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite passiveSprite;
    [SerializeField] private Vector2 disablePosition;
    [SerializeField] private Vector2 enabledPosition;
    [SerializeField] private float transitionDuration = 0.4f;
    [SerializeField] private Ease ease = Ease.InOutBack;
    [SerializeField] private UnityAction<bool> onValueChanged;

    private Tween transition;

    public void Setup(bool isEnabled, UnityAction<bool> onValueChanged)
    {
        value = isEnabled;
        icon.sprite = value ? activeSprite : passiveSprite;
        transition?.Kill();
        icon.rectTransform.anchoredPosition = value ? enabledPosition : disablePosition;
        this.onValueChanged = onValueChanged;
    }

    private void MoveKnob(Vector2 position)
    {
        transition?.Kill();
        transition = icon.rectTransform.DOAnchorPos(position, transitionDuration)
            .SetEase(ease)
            .SetUpdate(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        value = !value;
        icon.sprite = value ? activeSprite : passiveSprite;
        MoveKnob(value ? enabledPosition : disablePosition);
        onValueChanged?.Invoke(value);
    }
}
