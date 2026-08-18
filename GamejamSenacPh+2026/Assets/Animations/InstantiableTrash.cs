using DG.Tweening;
using UnityEngine;

public class InstantiableTrash : MonoBehaviour
{
    [SerializeField] private Transform visual;
    [SerializeField] private Transform shadow;

    [SerializeField] private float jumpPower = 1.5f;
    [SerializeField] private float duration = 0.6f;
    [SerializeField] private int jumpCount = 1;

    [SerializeField] private float shadowFollowSpeed = 10f;

    [SerializeField] private Vector3 launchSquash = new Vector3(1.3f, 0.6f, 1f);
    [SerializeField] private float squashDuration = 0.12f;
    [SerializeField] private float spinAmount = 360f;
    [SerializeField] private Vector3 landPunch = new Vector3(0.35f, -0.35f, 0f);
    [SerializeField] Vector3 shadowOffset;
    [SerializeField] float delayToStartFading;
    [SerializeField] float fadeDuration;

    private Vector3 landingPosition;
    private bool flying;

    public void Launch(Vector3 targetPosition)
    {
        landingPosition = targetPosition;
        flying = true;

        if (shadow != null)
            shadow.position = new Vector3(transform.position.x, landingPosition.y, transform.position.z);

        if (visual != null)
        {
            visual.localScale = launchSquash;
            visual.DOScale(Vector3.one, squashDuration).SetEase(Ease.OutBack);
            visual.DORotate(new Vector3(0f, 0f, spinAmount), duration, RotateMode.FastBeyond360).SetEase(Ease.Linear);
        }

        transform.DOJump(landingPosition, jumpPower, jumpCount, duration)
            .SetEase(Ease.Linear)
            .OnComplete(OnLanded);
    }

    private void Update()
    {
        if (shadow == null) return;

        Vector3 shadowPos = shadow.position;
        shadowPos.x = Mathf.Lerp(shadowPos.x, transform.position.x, Time.deltaTime * shadowFollowSpeed);
        shadowPos.y = landingPosition.y + shadowOffset.y;
        shadow.position = shadowPos;
    }
    void Start()
    {
        visual.GetComponent<SpriteRenderer>().DOFade(0f, fadeDuration).SetDelay(delayToStartFading);
        shadow.GetComponent<SpriteRenderer>().DOFade(0f, fadeDuration).SetDelay(delayToStartFading);
    }
    private void OnLanded()
    {
        flying = false;

        if (visual != null)
            visual.DOPunchScale(landPunch, 0.35f, 8, 0.8f);
    }
}
