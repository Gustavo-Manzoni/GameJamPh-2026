using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class CutsceneCinematicJuice : MonoBehaviour
{
    
    [SerializeField] Image topBar;
    [SerializeField] Image bottomBar;

   
    [SerializeField] Image characterImage;
    [SerializeField] CutsceneCharacter characterOwner = CutsceneCharacter.Pai;

 
    [SerializeField] Image secondCharacterImage;
    [SerializeField] CutsceneCharacter secondCharacterOwner = CutsceneCharacter.Filho;

    [Header("Aparecer/Desaparecer (escala 1/0, separado da respiracao)")]
    [SerializeField] Transform paiAppearPivot;
    [SerializeField] Transform filhoAppearPivot;
    [SerializeField] float appearScaleDuration = 0.35f;
    [SerializeField] Ease appearScaleEase = Ease.OutBack;
    [SerializeField] Ease disappearScaleEase = Ease.InBack;

    [SerializeField] RectTransform dialoguePanel;
    [SerializeField] CanvasGroup canvasGroup;

  
    [SerializeField] float topBarHeight = 105f;
    [SerializeField] float bottomBarHeight = 105f;
    [SerializeField] float barWaveAmount = 3f;
    [SerializeField] float barWaveSpeed = 1.7f;
    [SerializeField] float revealStiffness = 115f;
    [SerializeField] float revealDamping = 12f;

  
    [SerializeField] float characterBreathAmount = 0.025f;
    [SerializeField] float characterFloatAmount = 10f;
    [SerializeField] float characterSwayAngle = 1.5f;
    [SerializeField] float characterIdleSpeed = 1.25f;

    [Header("Destaque de quem esta falando")]
    [SerializeField] float speakingScaleBoost = 0.08f;
    [SerializeField] float notSpeakingScaleShrink = 0.08f;
    [SerializeField] float speakingStiffness = 90f;
    [SerializeField] float speakingDamping = 10f;

   
    [SerializeField] float dialogueFloatAmount = 4f;
    [SerializeField] float dialoguePulseAmount = 0.012f;
    [SerializeField] bool startVisible = true;

    RectTransform topBarRect;
    RectTransform bottomBarRect;

    RectTransform characterRect;
    Vector3 characterBasePosition;
    Vector3 characterBaseScale;
    Quaternion characterBaseRotation;
    SpringFloat characterSpeakingSpring;

    RectTransform secondCharacterRect;
    Vector3 secondCharacterBasePosition;
    Vector3 secondCharacterBaseScale;
    Quaternion secondCharacterBaseRotation;
    SpringFloat secondCharacterSpeakingSpring;

    Vector3 dialogueBasePosition;
    Vector3 dialogueBaseScale;
    SpringFloat revealSpring;
    bool isVisible;
    CutsceneCharacter currentSpeaker = CutsceneCharacter.None;

    void Awake()
    {
        topBarRect = topBar != null ? topBar.rectTransform : null;
        bottomBarRect = bottomBar != null ? bottomBar.rectTransform : null;

        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();

        characterRect = CacheCharacterPose(characterImage, out characterBasePosition, out characterBaseRotation, out characterBaseScale);
        characterSpeakingSpring = new SpringFloat(0f) { stiffness = speakingStiffness, damping = speakingDamping };

        secondCharacterRect = CacheCharacterPose(secondCharacterImage, out secondCharacterBasePosition, out secondCharacterBaseRotation, out secondCharacterBaseScale);
        secondCharacterSpeakingSpring = new SpringFloat(0f) { stiffness = speakingStiffness, damping = speakingDamping };

        if (dialoguePanel != null)
        {
            dialogueBasePosition = dialoguePanel.anchoredPosition3D;
            dialogueBaseScale = dialoguePanel.localScale;
        }

        revealSpring = new SpringFloat(startVisible ? 1f : 0f)
        {
            stiffness = revealStiffness,
            damping = revealDamping
        };
        isVisible = startVisible;
        ApplyReveal(revealSpring.value, 0f);

     
        if (paiAppearPivot != null) paiAppearPivot.localScale = Vector3.zero;
        if (filhoAppearPivot != null) filhoAppearPivot.localScale = Vector3.zero;
    }

    static RectTransform CacheCharacterPose(Image image, out Vector3 basePosition, out Quaternion baseRotation, out Vector3 baseScale)
    {
        RectTransform rect = image != null ? image.rectTransform : null;
        basePosition = rect != null ? rect.anchoredPosition3D : Vector3.zero;
        baseRotation = rect != null ? rect.localRotation : Quaternion.identity;
        baseScale = rect != null ? rect.localScale : Vector3.one;
        return rect;
    }

    void Update()
    {
        float reveal = Mathf.Clamp01(revealSpring.Update(isVisible ? 1f : 0f, Time.unscaledDeltaTime));
        ApplyReveal(reveal, Time.unscaledTime);
    }

    public void ShowCinematic()
    {
        isVisible = true;
    }

    public void HideCinematic()
    {
        isVisible = false;
    }

    public void ToggleCinematic()
    {
        isVisible = !isVisible;
    }

    public void SetSpeaker(CutsceneCharacter speaker)
    {
        currentSpeaker = speaker;

        SetAppearPivot(paiAppearPivot, speaker == CutsceneCharacter.Pai);
        SetAppearPivot(filhoAppearPivot, speaker == CutsceneCharacter.Filho);
    }

    void SetAppearPivot(Transform pivot, bool visible)
    {
        if (pivot == null) return;

        pivot.DOKill();
        pivot.DOScale(visible ? Vector3.one : Vector3.zero, appearScaleDuration)
            .SetEase(visible ? appearScaleEase : disappearScaleEase);
    }

    void ApplyReveal(float reveal, float time)
    {
        if (canvasGroup != null)
            canvasGroup.alpha = reveal;

         float topWave = Mathf.Sin(time * barWaveSpeed) * barWaveAmount * reveal;
        float bottomWave = Mathf.Sin(time * barWaveSpeed + Mathf.PI * 0.72f) * barWaveAmount * reveal;
        SetBarHeight(topBarRect, Mathf.Max(0f, topBarHeight * reveal + topWave));
        SetBarHeight(bottomBarRect, Mathf.Max(0f, bottomBarHeight * reveal + bottomWave));

        AnimateCharacter(characterRect, characterBasePosition, characterBaseRotation, characterBaseScale,
            characterSpeakingSpring, currentSpeaker == characterOwner, reveal, time, GetInstanceID() * 0.13f);

        AnimateCharacter(secondCharacterRect, secondCharacterBasePosition, secondCharacterBaseRotation, secondCharacterBaseScale,
            secondCharacterSpeakingSpring, currentSpeaker == secondCharacterOwner, reveal, time, GetInstanceID() * 0.13f + 10f);

        if (dialoguePanel != null)
        {
            float pulse = Mathf.Sin(time * 1.5f + 0.7f) * dialoguePulseAmount;
            float floatY = Mathf.Sin(time * 1.15f) * dialogueFloatAmount;
            dialoguePanel.anchoredPosition3D = dialogueBasePosition + Vector3.up * (floatY * reveal);
            dialoguePanel.localScale = dialogueBaseScale * (1f + pulse * reveal);
        }
    }

    void AnimateCharacter(RectTransform rect, Vector3 basePosition, Quaternion baseRotation, Vector3 baseScale,
        SpringFloat speakingSpring, bool isSpeaking, float reveal, float time, float phaseSeed)
    {
        if (rect == null) return;

        float emphasisTarget = isSpeaking ? speakingScaleBoost : -notSpeakingScaleShrink;
        float emphasis = speakingSpring.Update(emphasisTarget, Time.unscaledDeltaTime);

        float phase = time * characterIdleSpeed + phaseSeed;
        float breath = Mathf.Sin(phase) * characterBreathAmount;
        float floatY = Mathf.Sin(phase * 0.83f + 0.5f) * characterFloatAmount;
        float sway = Mathf.Sin(phase * 0.61f) * characterSwayAngle;

        rect.anchoredPosition3D = basePosition + Vector3.up * (floatY * reveal);
        rect.localRotation = baseRotation * Quaternion.Euler(0f, 0f, sway * reveal);

        float scaleMultiplier = 1f + emphasis * reveal;
        rect.localScale = new Vector3(
            baseScale.x * (1f - breath * 0.45f * reveal) * scaleMultiplier,
            baseScale.y * (1f + breath * reveal) * scaleMultiplier,
            baseScale.z);
    }

    static void SetBarHeight(RectTransform bar, float height)
    {
        if (bar != null)
            bar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }
}

