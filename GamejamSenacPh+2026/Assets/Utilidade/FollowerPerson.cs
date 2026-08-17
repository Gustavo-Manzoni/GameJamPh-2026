using UnityEngine;

[RequireComponent(typeof(PositionRecorder))]
public class FollowerPerson : MonoBehaviour
{
    private enum State { Idle, Following }

    [SerializeField] private float positionStiffness = 140f;
    [SerializeField] private float positionDamping = 16f;
    [SerializeField] private float referenceSpeed = 4.5f;

    [SerializeField] private SquashStretch squashStretch;
    [SerializeField] private Transform visual;
    [SerializeField] private Vector2 joinSquashScale = new Vector2(1.5f, 0.4f);
    [SerializeField] private float walkBobAmplitude = 0.05f;
    [SerializeField] private float walkBobFrequency = 8f;
    [SerializeField] private float idleBreathAmplitude = 0.025f;
    [SerializeField] private float idleBreathSpeed = 2f;

    [SerializeField] private float facingRotationSpeed = 12f;

    private State state = State.Idle;
    private PositionRecorder recorder;
    private PositionRecorder target;
    private float followDistance;
    private SpringVector2 posSpring;
    private float facingTargetY = 0f;
    private float currentFacingY = 0f;
    private float bobTimer;
    private Vector2 lastPos;

    public PositionRecorder Recorder => recorder;
    public bool IsIdle => state == State.Idle;

    private void Awake()
    {
        recorder = GetComponent<PositionRecorder>();
        posSpring = new SpringVector2(transform.position)
        {
            stiffness = positionStiffness,
            damping = positionDamping
        };
        lastPos = transform.position;
    }
    void Start()
    {
        ServiceLocator.Get<FollowChainManager>().MaxChainCount++;       
    }
    public void JoinChain(PositionRecorder newTarget, float distanceBack)
    {
        target = newTarget;
        followDistance = distanceBack;
        state = State.Following;

        if (squashStretch != null)
            squashStretch.SnapTo(joinSquashScale);
    }

    public void SetTarget(PositionRecorder newTarget, float distanceBack)
    {
        target = newTarget;
        followDistance = distanceBack;
    }

    private void Update()
    {
        if (state == State.Idle)
        {
            IdleBreath();
            ApplyFacing();
            return;
        }

        FollowTarget();
    }

    private void FollowTarget()
    {
        if (target == null) return;

        Vector2 desired = target.GetPointAtDistance(followDistance);
        Vector2 pos = posSpring.Update(desired, Time.deltaTime);
        transform.position = pos;

        Vector2 delta = pos - lastPos;
        float speed = delta.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        lastPos = pos;

        if (Mathf.Abs(delta.x) > 0.01f)
            facingTargetY = delta.x > 0f ? 0f : 180f;

        ApplyFacing();

        if (visual == null) return;

        if (speed > 0.05f)
        {
            bobTimer += Time.deltaTime * walkBobFrequency * (speed / referenceSpeed);
            float bob = Mathf.Abs(Mathf.Sin(bobTimer)) * walkBobAmplitude;
            visual.localPosition = new Vector3(visual.localPosition.x, bob, visual.localPosition.z);
        }
        else
        {
            bobTimer = 0f;
        }
    }

    private void ApplyFacing()
    {
        if (visual == null) return;
        currentFacingY = Mathf.Lerp(currentFacingY, facingTargetY, Time.deltaTime * facingRotationSpeed);
        visual.localRotation = Quaternion.Euler(0f, currentFacingY, 0f);
    }

    private void IdleBreath()
    {
        if (visual == null) return;
        float y = Mathf.Sin(Time.time * idleBreathSpeed + GetInstanceID() * 0.1f) * idleBreathAmplitude;
        visual.localPosition = new Vector3(visual.localPosition.x, y, visual.localPosition.z);
    }
}