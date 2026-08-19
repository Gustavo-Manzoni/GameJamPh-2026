using UnityEngine;
using UnityEngine.UI;


public class CutsceneCinematicJuice : MonoBehaviour
{
    
    [SerializeField] Image topBar;
    [SerializeField] Image bottomBar;
    [SerializeField] Image characterImage;
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

   
    [SerializeField] float dialogueFloatAmount = 4f;
    [SerializeField] float dialoguePulseAmount = 0.012f;
    [SerializeField] bool startVisible = true;

    RectTransform topBarRect;
    RectTransform bottomBarRect;
    RectTransform characterRect;
    Vector3 characterBasePosition;
    Vector3 characterBaseScale;
    Quaternion characterBaseRotation;
    Vector3 dialogueBasePosition;
    Vector3 dialogueBaseScale;
    SpringFloat revealSpring;
    bool isVisible;

    void Awake()
    {
        topBarRect = topBar != null ? topBar.rectTransform : null;
        bottomBarRect = bottomBar != null ? bottomBar.rectTransform : null;
        characterRect = characterImage != null ? characterImage.rectTransform : null;

        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (characterRect != null)
        {
            characterBasePosition = characterRect.anchoredPosition3D;
            characterBaseScale = characterRect.localScale;
            characterBaseRotation = characterRect.localRotation;
        }
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

    void ApplyReveal(float reveal, float time)
    {
        if (canvasGroup != null)
            canvasGroup.alpha = reveal;

         float topWave = Mathf.Sin(time * barWaveSpeed) * barWaveAmount * reveal;
        float bottomWave = Mathf.Sin(time * barWaveSpeed + Mathf.PI * 0.72f) * barWaveAmount * reveal;
        SetBarHeight(topBarRect, Mathf.Max(0f, topBarHeight * reveal + topWave));
        SetBarHeight(bottomBarRect, Mathf.Max(0f, bottomBarHeight * reveal + bottomWave));

        if (characterRect != null)
        {
            float phase = time * characterIdleSpeed + GetInstanceID() * 0.13f;
            float breath = Mathf.Sin(phase) * characterBreathAmount;
            float floatY = Mathf.Sin(phase * 0.83f + 0.5f) * characterFloatAmount;
            float sway = Mathf.Sin(phase * 0.61f) * characterSwayAngle;

            characterRect.anchoredPosition3D = characterBasePosition + Vector3.up * (floatY * reveal);
            characterRect.localRotation = characterBaseRotation * Quaternion.Euler(0f, 0f, sway * reveal);
            characterRect.localScale = new Vector3(
                characterBaseScale.x * (1f - breath * 0.45f * reveal),
                characterBaseScale.y * (1f + breath * reveal),
                characterBaseScale.z);
        }

        if (dialoguePanel != null)
        {
            float pulse = Mathf.Sin(time * 1.5f + 0.7f) * dialoguePulseAmount;
            float floatY = Mathf.Sin(time * 1.15f) * dialogueFloatAmount;
            dialoguePanel.anchoredPosition3D = dialogueBasePosition + Vector3.up * (floatY * reveal);
            dialoguePanel.localScale = dialogueBaseScale * (1f + pulse * reveal);
        }
    }

    static void SetBarHeight(RectTransform bar, float height)
    {
        if (bar != null)
            bar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }
}
