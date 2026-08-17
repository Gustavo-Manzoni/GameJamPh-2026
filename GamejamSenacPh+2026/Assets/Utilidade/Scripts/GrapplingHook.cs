using UnityEngine;
using DG.Tweening;
public class GrapplingHook : MonoBehaviour
{
    [HideInInspector] public GrapplingGun gun;
    [HideInInspector] public float speed = 20f;
    [SerializeField] Vector3 rotationOffset = new Vector3(0f, 0f, 0f);
    [SerializeField] GameObject onAttatchParticle;
    [SerializeField] Vector3 particleRotationOffset = new Vector3(0f, 0f, 0f);

   
    [SerializeField] private bool enableCameraFeedback = true;
    [SerializeField] private float attachShakeDuration = 0.2f;
    [SerializeField] private float attachShakeStrength = 0.4f;
    [SerializeField] private float attachZoomPulseDuration = 0.25f;
    [SerializeField] private float attachZoomAmount = 3.8f;
    [SerializeField] private float retractionZoom = 5f;
    [SerializeField] private float retractionZoomDuration = 0.3f;

    [SerializeField] float onLaunchZoom, onLaunchZoomDuration = 0.1f;

    Rigidbody2D rb;
    bool isAttached;
    bool isReturning;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true; 
    }

   
    public void Launch(Vector2 direction)
    {
        isAttached = false;
        isReturning = false;
        gameObject.SetActive(true);
        rb.linearVelocity = direction.normalized * speed;
        RotateTowardsVelocity();

        
        if (enableCameraFeedback && CameraController.Instance != null)
        {
            CameraController.Instance.Zoom(onLaunchZoom, onLaunchZoomDuration);
        }
    }

    public void Retract()
    {
        isAttached = false;
        isReturning = true;

       
        if (enableCameraFeedback && CameraController.Instance != null)
        {
            CameraController.Instance.Zoom(retractionZoom, retractionZoomDuration);
        }
    }

    void Update()
    {
        if (isReturning)
        {
            Vector2 dirToGun = (Vector2)gun.gunTip.position - rb.position;
            rb.linearVelocity = dirToGun.normalized * speed;
            RotateTowardsVelocity();

            if (dirToGun.magnitude < 0.3f)
            {
                rb.linearVelocity = Vector2.zero;
                isReturning = false;
                gameObject.SetActive(false);
            }
        }
        else if (!isAttached)
        {
            RotateTowardsVelocity();
        }
    }

    void RotateTowardsVelocity()
    {
        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(rotationOffset + new Vector3(0f, 0f, angle));
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isAttached || isReturning) return;
        if (((1 << other.gameObject.layer) & gun.grappleableLayer) != 0)
        {
            isAttached = true;
            rb.linearVelocity = Vector2.zero;

            Vector2 hitPoint = other.ClosestPoint(transform.position);
            gun.OnHookAttached(hitPoint);
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            GameObject particle = Instantiate(onAttatchParticle, hitPoint, Quaternion.Euler(particleRotationOffset + new Vector3(transform.rotation.eulerAngles.z,0,0)));
            Destroy(particle, 3f);

            
            if (enableCameraFeedback && CameraController.Instance != null)
            {
                CameraController.Instance.Impact(attachShakeDuration, attachShakeStrength, attachZoomPulseDuration, attachZoomAmount);
            }
        }
    }
}
