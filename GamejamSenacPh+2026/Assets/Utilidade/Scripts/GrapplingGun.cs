using UnityEngine;
using DG.Tweening;


public class GrapplingGun : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;
    public Transform gunPivot;     
    public Transform gunTip;       
    public GrapplingHook hookPrefab;
    public RopeVisual ropeVisual; 

    [Header("Órbita da arma")]
    public float orbitRadius = 0.6f;

    [Header("Hook")]
    public float hookSpeed = 20f;
    public float maxHookDistance = 12f;
    [Tooltip("Layer dos objetos em que o hook pode prender")]
    public LayerMask grappleableLayer;

    [Header("Corda / Balanço")]
    public float reelSpeed = 5f;
    public float minRopeLength = 1f;
    public float swingForce = 15f;

    [Header("VFX & Feedback")]
    [SerializeField] private float gunRecoilAmount = 0.2f;
    [SerializeField] private float gunRecoilDuration = 0.1f;
    [SerializeField] private float lineRendererPulseScale = 1.3f;
    [SerializeField] private float lineRendererPulseDuration = 0.15f;

    GrapplingHook currentHook;
    DistanceJoint2D rope;
    Rigidbody2D playerRb;
    PlayerMovement playerMovement;

    bool isShooting;
    bool isAttached;
    Vector2 attachPoint;

    private Vector3 gunOriginalPos;
    private Tween gunRecoilTween;

    void Awake()
    {
        playerRb = player.GetComponent<Rigidbody2D>();
        playerMovement = player.GetComponent<PlayerMovement>();

        gunOriginalPos = gunPivot.localPosition;

        rope = player.gameObject.AddComponent<DistanceJoint2D>();
        rope.enabled = false;
        rope.autoConfigureDistance = false;
        rope.autoConfigureConnectedAnchor = false;
        rope.maxDistanceOnly = true; 

        currentHook = Instantiate(hookPrefab, gunTip.position, Quaternion.identity);
        currentHook.gun = this;
        currentHook.speed = hookSpeed;
        currentHook.gameObject.SetActive(false);
    }

    void Update()
    {
        OrbitGunAroundPlayer();
        HandleInput();
    }

    void LateUpdate()
    {
       
        if (currentHook.gameObject.activeSelf)
            ropeVisual.UpdateRope(gunTip.position, currentHook.transform.position, isAttached);
        else
            ropeVisual.Stop();
    }

    void OrbitGunAroundPlayer()
    {
        
        Vector2 targetPoint = isAttached
            ? attachPoint
            : (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 dir = (targetPoint - (Vector2)player.position).normalized;

        gunPivot.position = (Vector2)player.position + dir * orbitRadius;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        gunPivot.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void HandleInput()
    {
        
        if (Input.GetMouseButtonDown(0) && !isShooting && !isAttached)
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = (mouseWorldPos - (Vector2)gunTip.position).normalized;

            isShooting = true;
            currentHook.transform.position = gunTip.position;
            currentHook.Launch(dir);
            ropeVisual.Begin(gunTip.position, currentHook.transform.position);

           
            //  PlayGunRecoil();
        }

       
        if (Input.GetMouseButtonUp(0))
        {
            DetachRope();
            currentHook.Retract();
            isShooting = false;
        }

      
        if (isShooting && !isAttached)
        {
            float dist = Vector2.Distance(gunTip.position, currentHook.transform.position);
            if (dist >= maxHookDistance)
            {
                currentHook.Retract();
                isShooting = false;
            }
        }

        if (isAttached)
            HandleSwingAndReel();
    }

    void HandleSwingAndReel()
    {
        Vector2 toPlayer = (Vector2)player.position - attachPoint;
        float currentDistance = toPlayer.magnitude;
        if (currentDistance < 0.01f) return;

        Vector2 radialDir = toPlayer.normalized;
        Vector2 tangentDir = new Vector2(-radialDir.y, radialDir.x); 

       
        if (Input.GetKey(KeyCode.E))
        {
            float newDistance = Mathf.Max(minRopeLength, rope.distance - reelSpeed * Time.deltaTime);
            ApplyAngularMomentum(rope.distance, newDistance, radialDir, tangentDir);
            rope.distance = newDistance;
        }

        
        if (Input.GetKey(KeyCode.Q))
        {
            float newDistance = Mathf.Min(maxHookDistance, rope.distance + reelSpeed * Time.deltaTime);
            ApplyAngularMomentum(rope.distance, newDistance, radialDir, tangentDir);
            rope.distance = newDistance;
        }

         float swingInput = 0f;
        if (Input.GetKey(KeyCode.A)) swingInput = -1f;
        if (Input.GetKey(KeyCode.D)) swingInput = 1f;

        if (swingInput != 0f)
            playerRb.AddForce(tangentDir * swingInput * swingForce);
    }

    
    void ApplyAngularMomentum(float oldDistance, float newDistance, Vector2 radialDir, Vector2 tangentDir)
    {
        if (oldDistance <= 0.01f || newDistance <= 0.01f) return;

        float tangentialSpeed = Vector2.Dot(playerRb.linearVelocity, tangentDir);
        float radialSpeed = Vector2.Dot(playerRb.linearVelocity, radialDir);

        float ratio = oldDistance / newDistance;
        float newTangentialSpeed = tangentialSpeed * ratio;

        playerRb.linearVelocity = radialDir * radialSpeed + tangentDir * newTangentialSpeed;
    }

    void PlayGunRecoil()
    {
        gunRecoilTween?.Kill();
        gunRecoilTween = DOTween.Sequence()
            .Append(gunPivot.DOLocalMove(gunOriginalPos - (Vector3)gunPivot.localPosition.normalized * gunRecoilAmount, gunRecoilDuration * 0.5f).SetEase(Ease.OutQuad))
            .Append(gunPivot.DOLocalMove(gunOriginalPos, gunRecoilDuration * 0.5f).SetEase(Ease.InQuad));

      
        if (ropeVisual != null && ropeVisual.GetComponent<LineRenderer>() != null)
        {
            LineRenderer lr = ropeVisual.GetComponent<LineRenderer>();
            Tween linePulse = DOTween.To(
                () => lr.widthMultiplier,
                x => lr.widthMultiplier = x,
                lineRendererPulseScale,
                lineRendererPulseDuration * 0.5f
            ).SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                DOTween.To(
                    () => lr.widthMultiplier,
                    x => lr.widthMultiplier = x,
                    1f,
                    lineRendererPulseDuration * 0.5f
                ).SetEase(Ease.InQuad);
            });
        }
    }

    
    public void OnHookAttached(Vector2 hitPoint)
    {
        isAttached = true;
        isShooting = false;
        attachPoint = hitPoint;

        rope.connectedBody = null;
        rope.connectedAnchor = attachPoint;
        rope.distance = Vector2.Distance(player.position, attachPoint);
        rope.enabled = true;

        if (playerMovement != null)
            playerMovement.isSwinging = true;

        
        PlayAttachFeedback();
    }

    void PlayAttachFeedback()
    {
        
        if (ropeVisual != null && ropeVisual.GetComponent<LineRenderer>() != null)
        {
            LineRenderer lr = ropeVisual.GetComponent<LineRenderer>();
            DOTween.Sequence()
                .Append(DOTween.To(
                    () => lr.widthMultiplier,
                    x => lr.widthMultiplier = x,
                    lineRendererPulseScale * 1.5f,
                    0.1f
                ).SetEase(Ease.OutQuad))
                .Append(DOTween.To(
                    () => lr.widthMultiplier,
                    x => lr.widthMultiplier = x,
                    1f,
                    0.15f
                ).SetEase(Ease.InQuad));
        }
    }

    void DetachRope()
    {
        isAttached = false;
        rope.enabled = false;

        if (playerMovement != null)
            playerMovement.isSwinging = false;
    }
}