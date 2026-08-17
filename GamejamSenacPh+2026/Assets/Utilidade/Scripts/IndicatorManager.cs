using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Events;
public class IndicatorManager : MonoBehaviour
{
    [SerializeField] private RectTransform indicator;
    [SerializeField] private Image indicatorImage;
    [SerializeField] private float rotationSmoothness = 8f;
    [SerializeField] private float scaleSmoothness = 8f;
    [SerializeField] private float colorSmoothness = 8f;
[SerializeField] private float positionSmoothness = 8f;
    private RectTransform currentParent;
    private Vector2 targetSize;
    private Vector2 targetOffset;
    private Color targetColor;
    private Tween colorTween;
    [SerializeField] UnityEvent onStart;

    
    private void Awake()
    {
        ServiceLocator.Register(this);
        
    }
    void Start()
    {

        onStart?.Invoke();
    }

    public void SelectRect(RectTransform target, Vector2 offset, Vector2 size, Color color)
    {
        
        if (currentParent != target)
        {
            indicator.SetParent(target);
            currentParent = target;
        }

       
        targetOffset = offset;

        
        indicator.sizeDelta = size;
        targetSize = size;

        
        targetColor = color;

        
        colorTween?.Kill();
        colorTween = indicatorImage.DOColor(targetColor, 1f / colorSmoothness);
    }

    public void Deselect(Color deselectionColor)
    {
        targetColor = deselectionColor;

        colorTween?.Kill();
        colorTween = indicatorImage.DOColor(targetColor, 1f / colorSmoothness);
    }

    public void GoToButton(MenuButtonFX button)
    {
        

        RectTransform buttonVisual = button.GetVisual();
        Vector2 offset = button.GetIndicatorOffset();
        Vector2 size = button.GetIndicatorSize();
        Color color = button.GetDeselectionColor();

        SelectRect(buttonVisual, offset, size, color);
    }

    public void GoToCard(CardTiltHover card)
    {
        
        RectTransform cardVisual = card.GetVisual();
        Vector2 offset = card.GetIndicatorOffset();
        Vector2 size = card.GetIndicatorSize();
        Color color = card.GetSelectionColor();

        SelectRect(cardVisual, offset, size, color);
    }

    private void Update()
    {
        
        indicator.localScale = Vector3.one;

        
        indicator.localRotation = Quaternion.Slerp(
            indicator.localRotation, Quaternion.identity, Time.deltaTime * rotationSmoothness);

        
        indicator.anchoredPosition = Vector2.Lerp(
            indicator.anchoredPosition, targetOffset, Time.deltaTime * positionSmoothness);

        
        indicator.sizeDelta = Vector2.Lerp(
            indicator.sizeDelta, targetSize, Time.deltaTime * scaleSmoothness);
    }
}
