using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PositionRecorder))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 6f;
    [SerializeField] private float acceleration = 45f;
    [SerializeField] private float deceleration = 55f;

    [SerializeField] private SquashStretch squashStretch;
    [SerializeField] private Vector2 startMoveKick = new Vector2(-0.15f, 0.15f);
    [SerializeField] private Vector2 stopMoveKick = new Vector2(0.22f, -0.22f);

    [SerializeField] private Transform visual;
    [SerializeField] private float bobAmplitude = 0.05f;
    [SerializeField] private float bobFrequency = 9f;

    [SerializeField] private float maxLeanAngle = 12f;
    [SerializeField] private float leanStiffness = 120f;
    [SerializeField] private float leanDamping = 12f;

    [SerializeField] private float facingRotationSpeed = 12f;

    private Rigidbody2D rb;
    private Vector2 input;
    private Vector2 velocity;
    private SpringFloat leanSpring;
    private float facingTargetY = 0f;
    private float currentFacingY = 0f;
    private float bobTimer;
    private bool wasMoving;
    bool canMove = true;

    public bool CanMove { get => canMove; set => canMove = value; }




    [SerializeField] private float positionStiffness = 140f;
    [SerializeField] private float positionDamping = 16f;
    [SerializeField] private float referenceSpeed = 4.5f;

    [SerializeField] private float walkBobAmplitude = 0.05f;
    [SerializeField] private float walkBobFrequency = 8f;
    [SerializeField] private float idleBreathAmplitude = 0.025f;
    [SerializeField] private float idleBreathSpeed = 2f;


    [SerializeField] private Transform torsoPivot;
    [SerializeField] private Transform neckPivot;

    [SerializeField] private float torsoSwayAmount = 6f;
    [SerializeField] private float torsoSwayStiffness = 140f;
    [SerializeField] private float torsoSwayDamping = 9f;

    [SerializeField] private float headSwayStiffness = 70f;
    [SerializeField] private float headSwayDamping = 6f;

    [SerializeField] private float idleSwayAmount = 3f;
    [SerializeField] private float idleSwaySpeed = 1.3f;
    [SerializeField] private float torsoStepSquash = 0.06f;

    private float followDistance;
    private Vector2 lastPos;

    private SpringFloat torsoSwaySpring;
    private SpringFloat headSwaySpring;
    private Vector3 torsoBaseScale;
   

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        leanSpring = new SpringFloat(0f) { stiffness = leanStiffness, damping = leanDamping };
        ServiceLocator.Register(this);


        lastPos = transform.position;


        torsoSwaySpring = new SpringFloat(0f) { stiffness = torsoSwayStiffness, damping = torsoSwayDamping };
        headSwaySpring = new SpringFloat(0f) { stiffness = headSwayStiffness, damping = headSwayDamping };

        if (torsoPivot != null)
            torsoBaseScale = torsoPivot.localScale;
 }

    private void Update()
    {
        UpdateBodySway(0);
        if(!canMove)
        {
            input = Vector2.zero;
            return;
        }
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        input = Vector2.ClampMagnitude(input, 1f);

        ApplyVisualJuice();

    }

    private void FixedUpdate()
    {
        Vector2 target = input * maxSpeed;
        float rate = input.sqrMagnitude > 0.01f ? acceleration : deceleration;
        velocity = Vector2.MoveTowards(velocity, target, rate * Time.fixedDeltaTime);
        rb.linearVelocity = velocity;

        bool isMoving = velocity.magnitude > 0.15f;
        if (isMoving && !wasMoving && squashStretch != null) squashStretch.Kick(startMoveKick);
        if (!isMoving && wasMoving && squashStretch != null) squashStretch.Kick(stopMoveKick);
        wasMoving = isMoving;
    }
    private void UpdateBodySway(float speed)
    {
        float swayTarget = speed > 0.05f
            ? Mathf.Sin(bobTimer) * torsoSwayAmount
            : Mathf.Sin(Time.time * idleSwaySpeed + GetInstanceID() * 0.1f) * idleSwayAmount;

        float torsoAngle = torsoSwaySpring.Update(swayTarget, Time.deltaTime);
        float headAngle = headSwaySpring.Update(torsoAngle, Time.deltaTime);

        if (torsoPivot != null)
        {
            torsoPivot.localRotation = Quaternion.Euler(0f, 0f, torsoAngle);

            float compress = Mathf.Abs(Mathf.Sin(bobTimer)) * torsoStepSquash;
            torsoPivot.localScale = new Vector3(
                torsoBaseScale.x * (1f + compress),
                torsoBaseScale.y * (1f - compress),
                torsoBaseScale.z);
        }

        if (neckPivot != null)
            neckPivot.localRotation = Quaternion.Euler(0f, 0f, headAngle);
    }
    private void ApplyVisualJuice()
    {
        if (Mathf.Abs(velocity.x) > 0.05f)
            facingTargetY = velocity.x > 0f ? 0f : 180f;

        float leanTarget = Mathf.Clamp(-input.x, -1f, 1f) * maxLeanAngle;
        float lean = leanSpring.Update(leanTarget, Time.deltaTime);
        currentFacingY = Mathf.Lerp(currentFacingY, facingTargetY, Time.deltaTime * facingRotationSpeed);

        if (visual == null) return;
        visual.localRotation = Quaternion.Euler(0f, currentFacingY, lean);

        float speed = velocity.magnitude;
        if (speed > 0.15f)
        {
            bobTimer += Time.deltaTime * bobFrequency * (speed / maxSpeed);
            float bob = Mathf.Abs(Mathf.Sin(bobTimer)) * bobAmplitude;
            visual.localPosition = new Vector3(visual.localPosition.x, bob, visual.localPosition.z);
        }
        else
        {
            bobTimer = 0f;
        }
    }
}