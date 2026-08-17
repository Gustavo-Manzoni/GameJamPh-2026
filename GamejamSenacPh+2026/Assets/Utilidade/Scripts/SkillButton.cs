    using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;

public class SkillButton : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler,
    IPointerClickHandler//,
    // ISelectHandler, IDeselectHandler,
    // ISubmitHandler
{
    [SerializeField] private Transform visualRoot;
    [SerializeField] private Transform buttonVisual;

    [SerializeField] private float   punchRotationZ  = -3f;
    [SerializeField] private float   punchDuration   = .3f;
    [SerializeField] private int     punchVibrato    = 3;
    [SerializeField] private float   punchElasticity = 59f;
    [SerializeField] private Vector3 punchScale      = new Vector3(0,0, 0f);
    [SerializeField] private float   punchScaleDur   = 1f;
    [SerializeField] private int     punchScaleVib   = 1;
    [SerializeField] private float   punchScaleEla   = 0.3f;

    [SerializeField] private Vector3 rootHoverScale = new Vector3(.1f, .1f, 1f);
    [SerializeField] private float   rootHoverIn    = 0.2f;
    [SerializeField] private float   rootHoverOut   = 0.15f;
    [SerializeField] private Ease    rootHoverEase  = Ease.OutBack;

    [Space]
    [SerializeField] private Vector3 visualClickScale = new Vector3(0.93f, 0.93f, 1f);
    [SerializeField] private float   visualClickDown  = 0.07f;
    [SerializeField] private float   visualClickUp    = 0.12f;
    [SerializeField] private Ease    visualClickEase  = Ease.OutQuad;

    public UnityEvent onClicked;
    public UnityEvent onSelection;
    public UnityEvent onDeselection;

    private Vector3 _restScale;
    private Vector3 _restRotation;
    private Vector3 _visualRestLocalRotation;
    private Vector3 _rootRestScale;
    private Vector3 _visualRestScale;

    private Tween _punchTween;
    private Tween _rootHoverTween;
    private Tween _visualClickTween;

    private bool _isHovering;

    private void Awake()
    {
        _restScale       = transform.localScale;
        _restRotation    = transform.localEulerAngles;
        _rootRestScale   = visualRoot.localScale;
        _visualRestScale = buttonVisual != null ? buttonVisual.localScale : Vector3.one;
        _visualRestLocalRotation = buttonVisual.localEulerAngles;
    }

    public void OnPointerEnter(PointerEventData _) => EnterHover();
   // public void OnSelect(BaseEventData _)          => EnterHover();
    public void OnPointerExit(PointerEventData _)  => ExitHover();
    //public void OnDeselect(BaseEventData _)        => ExitHover();

    private void EnterHover()
    {
       // SoundManager.Instance.Play(SFX.ButtonHover);
        _isHovering = true;

       
        _punchTween?.Kill();
        transform.localScale       = _restScale;
        buttonVisual.localEulerAngles = _visualRestLocalRotation    ;

        _punchTween = DOTween.Sequence()
            .Insert(0f, transform.DOPunchScale(
                punchScale, punchScaleDur, punchScaleVib, punchScaleEla))
            .Insert(0f, buttonVisual.DOPunchRotation(
                new Vector3(0f, 0f, punchRotationZ),
                punchDuration, punchVibrato, punchElasticity))
            .SetAutoKill(true);

        _rootHoverTween?.Kill();
        visualRoot.localScale = _rootRestScale;

        _rootHoverTween = visualRoot
            .DOScale(rootHoverScale + _rootRestScale, rootHoverIn)
            .SetEase(rootHoverEase);

        onSelection?.Invoke();
    }

    private void ExitHover()
    {
       // SoundManager.Instance.Play(SFX.ButtonUnhover);
        _isHovering = false;

        _rootHoverTween?.Kill();
        visualRoot.localScale = rootHoverScale + _rootRestScale;

        _rootHoverTween = visualRoot
            .DOScale(_rootRestScale, rootHoverOut)
            .SetEase(Ease.InOutQuad);

        onDeselection?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
       
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (buttonVisual == null) return;

        _visualClickTween?.Kill();
        buttonVisual.localScale = _visualRestScale;

        _visualClickTween = buttonVisual
            .DOScale(visualClickScale, visualClickDown)
            .SetEase(visualClickEase);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (buttonVisual == null) return;

        _visualClickTween?.Kill();
        buttonVisual.localScale = visualClickScale;

        _visualClickTween = buttonVisual
            .DOScale(_visualRestScale, visualClickUp)
            .SetEase(Ease.OutBack);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
      
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

       // SoundManager.Instance.Play(SFX.ButtonClick);
        onClicked?.Invoke();
    }

    // public void OnSubmit(BaseEventData _)
    // {
    //     SoundManager.Instance.Play(SFX.ButtonClick);
    //     onClicked?.Invoke();
    // }

    private void OnDisable()
    {
        _punchTween?.Kill();
        _rootHoverTween?.Kill();
        _visualClickTween?.Kill();
        transform.localScale       = _restScale;
        transform.localEulerAngles = _restRotation;
        if (visualRoot != null)    visualRoot.localScale    = _rootRestScale;
        if (buttonVisual != null)  buttonVisual.localScale  = _visualRestScale;
    }
}