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

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject angryFace;
    [SerializeField] private GameObject happyFace;

    [SerializeField] private GameObject trashPrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private float throwIntervalMin = 2f;
    [SerializeField] private float throwIntervalMax = 5f;
    [SerializeField] private float throwRadiusMin = 1f;
    [SerializeField] private float throwRadiusMax = 2.5f;

    private State state = State.Idle;
    private PositionRecorder recorder;
    private PositionRecorder target;
    private float followDistance;
    private SpringVector2 posSpring;
    private float facingTargetY = 0f;
    private float currentFacingY = 0f;
    private float bobTimer;
    private Vector2 lastPos;
    private float throwTimer;

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
        throwTimer = Random.Range(throwIntervalMin, throwIntervalMax);
       
    }
    void Start(){ ServiceLocator.Get<FollowChainManager>().MaxChainCount++;}

    public void JoinChain(PositionRecorder newTarget, float distanceBack)
    {
        target = newTarget;
        followDistance = distanceBack;
        state = State.Following;

        if (squashStretch != null)
            squashStretch.SnapTo(joinSquashScale);

        if (angryFace != null) angryFace.SetActive(false);
        if (happyFace != null) happyFace.SetActive(true);
    }

    public void SetTarget(PositionRecorder newTarget, float distanceBack)
    {
        target = newTarget;
        followDistance = distanceBack;
    }

    public void ReleaseFromChain()
    {
        target = null;
        state = State.Idle;
        throwTimer = Random.Range(throwIntervalMin, throwIntervalMax);

        if (angryFace != null) angryFace.SetActive(true);
        if (happyFace != null) happyFace.SetActive(false);
    }

    private void Update()
    {
        if (state == State.Idle)
        {
            IdleBreath();
            ApplyFacing();
            UpdateThrowTimer();

            if (animator != null)
                animator.SetBool("IsWalking", false);

            return;
        }

        FollowTarget();
    }

    private void UpdateThrowTimer()
    {
        throwTimer -= Time.deltaTime;
        if (throwTimer <= 0f)
            ThrowTrash();
    }

    private void ThrowTrash()
    {
        throwTimer = Random.Range(throwIntervalMin, throwIntervalMax);

        if (animator != null)
            animator.SetTrigger("Throw");

        if (trashPrefab == null) return;

        Vector3 spawnPoint = throwPoint != null ? throwPoint.position : transform.position;
        GameObject trashObj = Instantiate(trashPrefab, spawnPoint, Quaternion.identity);
        InstantiableTrash trash = trashObj.GetComponent<InstantiableTrash>();
        if (trash == null) return;

        Vector2 randomDir = Random.insideUnitCircle.normalized;
        float randomDist = Random.Range(throwRadiusMin, throwRadiusMax);
        Vector3 landing = spawnPoint + (Vector3)(randomDir * randomDist);
        trash.Launch(landing);
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

        if (animator != null)
            animator.SetBool("IsWalking", speed > 0.05f);

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
