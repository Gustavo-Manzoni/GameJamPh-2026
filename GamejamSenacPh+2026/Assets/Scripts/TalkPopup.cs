using DG.Tweening;
using TMPro;
using UnityEngine;

public class TalkPopup : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float popDuration = 0.35f;
    [SerializeField] private float floatHeight = 0.8f;
    [SerializeField] private float holdDuration = 0.8f;
    [SerializeField] private float fadeDuration = 0.4f;
    [SerializeField] private Ease popEase = Ease.OutBack;

    public void Show(string message, Color color)
    {
        if (text != null)
        {
            text.text = message;
            text.color = color;
        }

        transform.localScale = Vector3.zero;
        if (canvasGroup != null) canvasGroup.alpha = 1f;

        float totalFloatDuration = popDuration + holdDuration + fadeDuration;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(1f, popDuration).SetEase(popEase));
        sequence.Join(transform.DOPunchRotation(new Vector3(0f, 0f, 10f), popDuration, 6, 0.6f));
        sequence.Join(transform.DOMoveY(transform.position.y + floatHeight, totalFloatDuration).SetEase(Ease.OutSine));
        sequence.AppendInterval(holdDuration);

        if (canvasGroup != null)
            sequence.Append(canvasGroup.DOFade(0f, fadeDuration));
        else
            sequence.AppendInterval(fadeDuration);

        sequence.OnComplete(() => Destroy(gameObject));
    }
}
