using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class CardTiltHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    
    [SerializeField] RectTransform visual;
    [SerializeField] private float maxTiltAngle = 15f;
    [SerializeField] private float tiltSmoothness = 10f;

   
    [SerializeField] private float hoverScale = 1.05f;
    [SerializeField] private float liftAmount = 20f;
    [SerializeField] private float moveSmoothness = 8f;

    [SerializeField] private Vector2 indicatorOffset;
    [SerializeField] private Vector2 indicatorSize;
    [SerializeField] private Color selectionColor = Color.white;
    [SerializeField] private Color deselectionColor = Color.gray;

    private RectTransform rectTransform;
    private Vector3 initialScale;
    private Vector2 initialAnchoredPos;

    private Quaternion targetRotation = Quaternion.identity;
    private Vector3 targetScale;
    private Vector2 targetAnchoredPos;

    private bool isHovering = false;
    private IndicatorManager indicatorManager;

    public RectTransform GetVisual() => visual;
    public Vector2 GetIndicatorOffset() => indicatorOffset;
    public Vector2 GetIndicatorSize() => indicatorSize;
    public Color GetSelectionColor() => selectionColor;

    void Awake()
    {
        rectTransform = visual.GetComponent<RectTransform>();
        initialScale = rectTransform.localScale;
        initialAnchoredPos = rectTransform.anchoredPosition;
        targetScale = initialScale;
        targetAnchoredPos = initialAnchoredPos;

         }
    void Start(){  indicatorManager = ServiceLocator.Get<IndicatorManager>();
 }

    void Update()
    {
         rectTransform.localRotation = Quaternion.Slerp(
            rectTransform.localRotation, targetRotation, Time.deltaTime * tiltSmoothness);

        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale, targetScale, Time.deltaTime * moveSmoothness);

        rectTransform.anchoredPosition = Vector2.Lerp(
            rectTransform.anchoredPosition, targetAnchoredPos, Time.deltaTime * moveSmoothness);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        targetScale = initialScale * hoverScale;
        targetAnchoredPos = initialAnchoredPos + Vector2.up * liftAmount;

        indicatorManager.SelectRect(visual, indicatorOffset, indicatorSize, selectionColor);
        SoundManager.Instance.Play(SFX.CardHover);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (!isHovering) return;

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, eventData.position, eventData.enterEventCamera, out localPoint))
        {
            Vector2 normalized = new Vector2(
                localPoint.x / rectTransform.rect.width,
                localPoint.y / rectTransform.rect.height
            );

            float tiltZ = normalized.x * maxTiltAngle * 2f;

            targetRotation = Quaternion.Euler(0f, 0f, tiltZ);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        targetRotation = Quaternion.identity;
        targetScale = initialScale;
        targetAnchoredPos = initialAnchoredPos;

        indicatorManager.Deselect(deselectionColor);
        SoundManager.Instance.Play(SFX.CardUnhover);
    }
}