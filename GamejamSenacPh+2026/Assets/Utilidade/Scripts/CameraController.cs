using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    private Camera mainCamera;
    private Vector3 originalPos;
    private float originalZoom;

    private Tween shakeTween;
    private Tween zoomTween;
    private Tween positionTween;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        mainCamera = GetComponent<Camera>();
        originalPos = transform.position;
        originalZoom = mainCamera.orthographicSize;
    }

    public void Shake(float duration = 0.2f, float strength = 0.3f)
    {
        shakeTween?.Kill();
        shakeTween = transform.DOShakePosition(duration, strength, 10, 90f, false, true)
            .OnComplete(() => transform.position = originalPos);
    }

    public void Zoom(float targetZoom, float duration = 0.3f)
    {
        zoomTween?.Kill();
        zoomTween = DOTween.To(
            () => mainCamera.orthographicSize,
            x => mainCamera.orthographicSize = x,
            targetZoom,
            duration
        ).SetEase(Ease.OutQuad);
    }

    public void ResetZoom(float duration = 0.3f)
    {
        Zoom(originalZoom, duration);
    }
    public void MoveTowards(Vector3 targetPos, float duration = 0.3f, Ease easeType = Ease.OutQuad)
    {
        positionTween?.Kill();
        Vector3 target = new Vector3(targetPos.x, targetPos.y, originalPos.z);
        positionTween = transform.DOMove(target, duration).SetEase(easeType);
    }
    public void ResetPosition(float duration = 0.3f)
    {
        positionTween?.Kill();
        positionTween = transform.DOMove(originalPos, duration).SetEase(Ease.OutQuad);
    }
    public void ZoomPulse(float expandZoom, float duration = 0.2f)
    {
        zoomTween?.Kill();
        zoomTween = DOTween.Sequence()
            .Append(DOTween.To(
                () => mainCamera.orthographicSize,
                x => mainCamera.orthographicSize = x,
                expandZoom,
                duration * 0.5f
            ).SetEase(Ease.OutQuad))
            .Append(DOTween.To(
                () => mainCamera.orthographicSize,
                x => mainCamera.orthographicSize = x,
                originalZoom,
                duration * 0.5f
            ).SetEase(Ease.InQuad));
    }
    public void Impact(float shakeDuration = 0.15f, float shakeStrength = 0.4f, float zoomDuration = 0.2f, float expandZoom = 3.5f)
    {
        Shake(shakeDuration, shakeStrength);
        ZoomPulse(expandZoom, zoomDuration);
    }
}
