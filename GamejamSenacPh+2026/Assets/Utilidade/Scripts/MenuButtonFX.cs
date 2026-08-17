using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GameJam.Utilities;

[RequireComponent(typeof(RectTransform))]
public class MenuButtonFX : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler//, ISelectHandler, IDeselectHandler, ISubmitHandler
{
    [SerializeField] private RectTransform visual;
    [SerializeField] private RectTransform scaleTarget;
    [SerializeField] private Image buttonImage;
    [SerializeField] private TextMeshProUGUI label;

    [SerializeField] private float idleBobAmount = 6f;
    [SerializeField] private float idleBobDuration = 1.1f;

    [SerializeField] private float hoverScale = 1.08f;
    [SerializeField] private float hoverScaleDuration = 0.25f;

    [SerializeField] private float hoverLiftAmount = 10f;
    [SerializeField] private float hoverTiltAngle = 3f;
    [SerializeField] private float hoverMoveDuration = 0.25f;

    [SerializeField] private Color hoverButtonColor = new Color(1f, 0.85f, 0.3f);
    [SerializeField] private Color hoverTextColor = Color.black;
    [SerializeField] private float colorDuration = 0.2f;

    [SerializeField] private Vector3 clickPunchScale = new Vector3(0.15f, 0.15f, 0.15f);
    [SerializeField] private float clickPunchDuration = 0.3f;

    [SerializeField] private Vector2 indicatorOffset;
    [SerializeField] private Vector2 indicatorSize;
    [SerializeField] private Color selectionColor = Color.white;
    [SerializeField] private Color deselectionColor = Color.gray;

    private RectTransform _selfRect;
    private Button _button;

    private Vector2 _restAnchoredPosition;
    private Vector3 _restScale;
    private Color _baseButtonColor;
    private Color _baseTextColor;

    private Sequence _idleSequence;
    private Sequence _hoverSequence;
    private IndicatorManager _indicatorManager;

    public RectTransform GetVisual() => visual;
    public Vector2 GetIndicatorOffset() => indicatorOffset;
    public Vector2 GetIndicatorSize() => indicatorSize;
    public Color GetSelectionColor() => selectionColor;
    public Color GetDeselectionColor() => deselectionColor;
    CooldownTimer clickPunchCooldown;

    private void Awake()
    {
        _selfRect = visual;
        _button = GetComponent<Button>();
        _restAnchoredPosition = _selfRect.anchoredPosition;
        _restScale = scaleTarget.localScale;

        _baseButtonColor = buttonImage.color;
        _baseTextColor = label.color;
clickPunchCooldown = new CooldownTimer(clickPunchDuration);
          }
    void Start(){  _indicatorManager = ServiceLocator.Get<IndicatorManager>();
}
    void Update()
    {
        clickPunchCooldown.Tick();
    }
    private void OnEnable()
    {
        PlayIdleBob();
    }

    private void OnDisable()
    {
        _idleSequence?.Kill();
        _hoverSequence?.Kill();
    }
    private void PlayIdleBob()
    {

        _idleSequence?.Kill();
        float baseY = visual.anchoredPosition.y;
        
       float randomDelay = Random.Range(0f, idleBobDuration);
       float randomDuration = idleBobDuration * Random.Range(0.8f, 1.2f);

        _idleSequence = DOTween.Sequence();
        _idleSequence.PrependInterval(randomDelay);
        _idleSequence.Append(visual.DOAnchorPosY(baseY + idleBobAmount, randomDuration).SetEase(Ease.InOutSine));
        _idleSequence.Append(visual.DOAnchorPosY(baseY, randomDuration).SetEase(Ease.InOutSine));
        _idleSequence.SetLoops(-1);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_button.interactable) return;

        AnimateTo(hoverScale, _restAnchoredPosition + Vector2.up * hoverLiftAmount, hoverTiltAngle, hoverButtonColor, hoverTextColor);
        _indicatorManager.SelectRect(visual, indicatorOffset, indicatorSize, selectionColor);
        SoundManager.Instance.Play(SFX.ButtonHover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AnimateTo(1f, _restAnchoredPosition, 0f, _baseButtonColor, _baseTextColor);
        _indicatorManager.Deselect(deselectionColor);
        SoundManager.Instance.Play(SFX.ButtonUnhover);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_button.interactable) return;

        if(clickPunchCooldown.CanUse)
        {
            clickPunchCooldown.Use();
            _selfRect.DOPunchScale(clickPunchScale, clickPunchDuration, 6, 0.7f);
            SoundManager.Instance.Play(SFX.ButtonClick);
        }
        
        // _selfRect.DOPunchScale(clickPunchScale, clickPunchDuration, 6, 0.7f);
        // SoundManager.Instance.Play(SFX.ButtonClick);
    }

    private void AnimateTo(float scaleMultiplier, Vector2 anchoredPosition, float tiltAngle, Color buttonColor, Color textColor)
    {
        _hoverSequence?.Kill();
        _hoverSequence = DOTween.Sequence();

       
        _hoverSequence.Join(scaleTarget.DOScale(_restScale * scaleMultiplier, hoverScaleDuration).SetEase(Ease.OutBack, 1.4f));
        //_hoverSequence.Join(_selfRect.DOAnchorPos(anchoredPosition, hoverMoveDuration).SetEase(Ease.OutQuad));
       
        _hoverSequence.Join(visual.DOLocalRotate(new Vector3(0f, 0f, tiltAngle), hoverMoveDuration).SetEase(Ease.OutQuad));
        

        _hoverSequence.Join(buttonImage.DOColor(buttonColor, colorDuration));
        _hoverSequence.Join(label.DOColor(textColor, colorDuration));
    }

    // public void OnSelect(BaseEventData eventData)
    // {
    //    OnPointerEnter(eventData as PointerEventData);
    // }

    // public void OnDeselect(BaseEventData eventData)
    // {
    //     OnPointerExit(eventData as PointerEventData);
    // }

    // public void OnSubmit(BaseEventData eventData)
    // {
    //     OnPointerClick(eventData as PointerEventData);
    // }

}
