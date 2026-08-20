using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FaseManager : MonoBehaviour
{
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Timing")]
    [SerializeField] private float showDuration = 2f;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    [Header("Juice")]
    [SerializeField] private float punchScale = 0.35f;
    [SerializeField] private float punchDuration = 0.6f;
    [SerializeField] private int punchVibrato = 8;
    [SerializeField, Range(0f, 1f)] private float punchElasticity = 0.85f;
    [SerializeField] private Ease appearEase = Ease.OutBack;

    private void Start()
    {
        if (levelText == null) return;

        levelText.text = $"Nível {SceneManager.GetActiveScene().name}";

        Transform textTransform = levelText.transform;
        textTransform.localScale = Vector3.zero;
        levelText.alpha = 0f;
        if (canvasGroup != null) canvasGroup.alpha = 0f;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(textTransform.DOScale(1f, fadeInDuration).SetEase(appearEase));
        sequence.Join(levelText.DOFade(1f, fadeInDuration));
        if (canvasGroup != null)
            sequence.Join(canvasGroup.DOFade(1f, fadeInDuration));

        sequence.Append(textTransform.DOPunchScale(Vector3.one * punchScale, punchDuration, punchVibrato, punchElasticity));
        sequence.AppendInterval(showDuration);

        sequence.Append(levelText.DOFade(0f, fadeOutDuration));
        sequence.Join(textTransform.DOScale(0.7f, fadeOutDuration).SetEase(Ease.InBack));
        if (canvasGroup != null)
            sequence.Join(canvasGroup.DOFade(0f, fadeOutDuration));

        sequence.OnComplete(() => levelText.gameObject.SetActive(false));
    }
}
