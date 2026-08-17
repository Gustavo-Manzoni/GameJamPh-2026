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


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        leanSpring = new SpringFloat(0f) { stiffness = leanStiffness, damping = leanDamping };
        ServiceLocator.Register(this);
 }

    private void Update()
    {
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