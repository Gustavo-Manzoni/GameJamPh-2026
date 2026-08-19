using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RobotPatrol : MonoBehaviour
{
    [SerializeField] private Vector2[] patrolMoves =
    {
        new Vector2(0f, -3f),
        new Vector2(-10f, 0f),
        new Vector2(0f, 3f),
        new Vector2(10f, 0f)
    };
    [SerializeField, Min(0.1f)] private float moveSpeed = 4.5f;
    [SerializeField, Min(0f)] private float cornerPause = 0.12f;
    [SerializeField, Min(0.001f)] private float waypointTolerance = 0.02f;

    [Header("Visual setup")]
    [SerializeField] private Transform visual;
    [SerializeField] private Transform hipsPivot;
    [SerializeField] private Transform headPivot;
    [SerializeField] private Transform leftArmPivot;
    [SerializeField] private Transform rightArmPivot;
    [SerializeField] private Transform antennaPivot;
    [SerializeField] private Transform legPivot;
    [SerializeField] private SquashStretch squashStretch;

    [Header("Visual feel")]
    [SerializeField] private float bobAmplitude = 0.045f;
    [SerializeField] private float bobFrequency = 9f;
    [SerializeField] private float leanAngle = 10f;
    [SerializeField] private float visualResponsiveness = 14f;
    [SerializeField] private float torsoSwayAmount = 7f;
    [SerializeField] private float hipsSwayAmount = 5f;
    [SerializeField] private float headSwayAmount = 6f;
    [SerializeField] private float armSwayAmount = 15f;
    [SerializeField] private float antennaSwayAmount = 12f;
    [SerializeField] private float legSwingAmount = 18f;
    [SerializeField] private float idleBreathSpeed = 1.8f;
    [SerializeField] private float idleBreathAmount = 0.02f;
    [SerializeField] private float armLeadLag = 0.8f;
    [SerializeField] private float torsoStiffness = 120f;
    [SerializeField] private float torsoDamping = 11f;
    [SerializeField] private float hipsStiffness = 100f;
    [SerializeField] private float hipsDamping = 10f;
    [SerializeField] private float headStiffness = 110f;
    [SerializeField] private float headDamping = 12f;
    [SerializeField] private float armStiffness = 90f;
    [SerializeField] private float armDamping = 8f;
    [SerializeField] private float antennaStiffness = 80f;
    [SerializeField] private float antennaDamping = 7f;
    [SerializeField] private float legStiffness = 110f;
    [SerializeField] private float legDamping = 9f;
    [SerializeField, Range(0f, 1f)] private float hitCameraTrauma = 0.08f;

    private Rigidbody2D body;
    private Vector2 routeOrigin;
    private Vector2 waypoint;
    private Vector2 velocity;
    private Vector3 visualBasePosition;
    private Vector3 legBasePosition;
    private int moveIndex;
    private float pauseTimer;
    private float bobTimer;
    private float facingY;

    private SpringFloat hipsSpring;
    private SpringFloat headSpring;
    private SpringFloat leftArmSpring;
    private SpringFloat rightArmSpring;
    private SpringFloat antennaSpring;
    private SpringFloat legSpring;
    private SpringFloat leanSpring;

    private Quaternion headBaseRotation;
    private Quaternion hipsBaseRotation;
    private Quaternion leftArmBaseRotation;
    private Quaternion rightArmBaseRotation;
    private Quaternion antennaBaseRotation;
    private Quaternion legBaseRotation;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = true;
        body.bodyType = RigidbodyType2D.Kinematic;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        routeOrigin = body.position;
        if (visual != null)
            visualBasePosition = visual.localPosition;

        if (hipsPivot != null) hipsBaseRotation = hipsPivot.localRotation;
        if (headPivot != null) headBaseRotation = headPivot.localRotation;
        if (leftArmPivot != null) leftArmBaseRotation = leftArmPivot.localRotation;
        if (rightArmPivot != null) rightArmBaseRotation = rightArmPivot.localRotation;
        if (antennaPivot != null) antennaBaseRotation = antennaPivot.localRotation;
        if (legPivot != null)
        {
            legBaseRotation = legPivot.localRotation;
            legBasePosition = legPivot.localPosition;
        }

        hipsSpring = new SpringFloat(0f) { stiffness = hipsStiffness, damping = hipsDamping };
        headSpring = new SpringFloat(0f) { stiffness = headStiffness, damping = headDamping };
        leftArmSpring = new SpringFloat(0f) { stiffness = armStiffness, damping = armDamping };
        rightArmSpring = new SpringFloat(0f) { stiffness = armStiffness, damping = armDamping };
        antennaSpring = new SpringFloat(0f) { stiffness = antennaStiffness, damping = antennaDamping };
        legSpring = new SpringFloat(0f) { stiffness = legStiffness, damping = legDamping };
        leanSpring = new SpringFloat(0f) { stiffness = 110f, damping = 12f };

        SetWaypoint();
    }

    private void FixedUpdate()
    {
        if (patrolMoves == null || patrolMoves.Length == 0) return;

        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.fixedDeltaTime;
            velocity = Vector2.zero;
            return;
        }

        Vector2 position = body.position;
        Vector2 toWaypoint = waypoint - position;
        float step = moveSpeed * Time.fixedDeltaTime;

        if (toWaypoint.sqrMagnitude <= waypointTolerance * waypointTolerance || toWaypoint.magnitude <= step)
        {
            body.MovePosition(waypoint);
            moveIndex = (moveIndex + 1) % patrolMoves.Length;
            SetWaypoint();
            pauseTimer = cornerPause;
            velocity = Vector2.zero;
            return;
        }

        velocity = toWaypoint.normalized * moveSpeed;
        body.MovePosition(position + velocity * Time.fixedDeltaTime);
    }

    private void Update()
    {
        float speed01 = velocity.magnitude / moveSpeed;
        if (speed01 > 0.01f)
            bobTimer += Time.deltaTime * bobFrequency * speed01;

        if (Mathf.Abs(velocity.x) > 0.01f)
            facingY = velocity.x > 0f ? 0f : 180f;

        float bob = Mathf.Abs(Mathf.Sin(bobTimer)) * bobAmplitude * Mathf.Clamp01(speed01);
        float leanTarget = (-velocity.x / Mathf.Max(moveSpeed, 0.0001f)) * leanAngle;
        float lean = leanSpring.Update(leanTarget, Time.deltaTime);

        if (visual != null)
        {
            visual.localPosition = Vector3.Lerp(
                visual.localPosition,
                visualBasePosition + Vector3.up * bob,
                Time.deltaTime * visualResponsiveness);

            visual.localRotation = Quaternion.Lerp(
                visual.localRotation,
                Quaternion.Euler(0f, facingY, lean),
                Time.deltaTime * visualResponsiveness);
        }

        AnimateRig(speed01, bobTimer);
    }

    private void AnimateRig(float speed01, float phase)
    {
        float movementBlend = Mathf.Clamp01(speed01 * 1.8f);
        float swayPhase = phase + 0.7f;

        float hipsTarget = Mathf.Sin(swayPhase * 0.9f + 1.2f) * hipsSwayAmount * (0.25f + movementBlend);
        float hipsAngle = hipsSpring.Update(hipsTarget, Time.deltaTime);
        if (hipsPivot != null)
            hipsPivot.localRotation = hipsBaseRotation * Quaternion.Euler(0f, 0f, hipsAngle);

        float headTarget = Mathf.Sin(swayPhase * 1.2f + 0.8f) * headSwayAmount * (0.2f + movementBlend);
        float headAngle = headSpring.Update(headTarget, Time.deltaTime);
        if (headPivot != null)
            headPivot.localRotation = headBaseRotation * Quaternion.Euler(0f, 0f, headAngle);

        float leftArmTarget = Mathf.Sin(swayPhase + armLeadLag) * armSwayAmount * (0.35f + movementBlend);
        float rightArmTarget = Mathf.Sin(swayPhase + Mathf.PI - armLeadLag) * armSwayAmount * (0.35f + movementBlend);
        float leftArmAngle = leftArmSpring.Update(leftArmTarget, Time.deltaTime);
        float rightArmAngle = rightArmSpring.Update(rightArmTarget, Time.deltaTime);
        if (leftArmPivot != null)
            leftArmPivot.localRotation = leftArmBaseRotation * Quaternion.Euler(0f, 0f, leftArmAngle);
        if (rightArmPivot != null)
            rightArmPivot.localRotation = rightArmBaseRotation * Quaternion.Euler(0f, 0f, rightArmAngle);

        float antennaTarget = Mathf.Sin(swayPhase * 1.6f) * antennaSwayAmount * (0.25f + movementBlend);
        float antennaAngle = antennaSpring.Update(antennaTarget, Time.deltaTime);
        if (antennaPivot != null)
            antennaPivot.localRotation = antennaBaseRotation * Quaternion.Euler(0f, 0f, antennaAngle);

        float legTarget = Mathf.Sin(swayPhase * 1.5f + 0.6f) * legSwingAmount * (0.2f + movementBlend);
        float legAngle = legSpring.Update(legTarget, Time.deltaTime);
        if (legPivot != null)
        {
            legPivot.localRotation = legBaseRotation * Quaternion.Euler(0f, 0f, legAngle);
            legPivot.localPosition = legBasePosition + Vector3.up * (Mathf.Sin(phase * 1.7f) * idleBreathAmount + Mathf.Abs(Mathf.Sin(phase)) * 0.015f * movementBlend);
        }
    }

    public void OnFollowerDetected(FollowerPerson follower)
    {
        FollowChainManager chainManager = FollowChainManager.Instance;
        if (chainManager == null || !chainManager.ReleaseFromFollowerAndBehind(follower, body.position))
            return;

        if (squashStretch != null)
            squashStretch.Kick(new Vector2(-0.28f, 0.4f));

        CameraShake.Instance?.AddTrauma(hitCameraTrauma);
    }

    private void SetWaypoint()
    {
        if (patrolMoves == null || patrolMoves.Length == 0) return;

        Vector2 offset = Vector2.zero;
        for (int i = 0; i <= moveIndex; i++)
            offset += patrolMoves[i];
        waypoint = routeOrigin + offset;
    }

    private void OnDrawGizmosSelected()
    {
        if (patrolMoves == null || patrolMoves.Length == 0) return;

        Vector3 point = Application.isPlaying ? (Vector3)routeOrigin : transform.position;
        Gizmos.color = new Color(1f, 0.45f, 0.1f, 0.85f);
        for (int i = 0; i < patrolMoves.Length; i++)
        {
            Vector3 next = point + (Vector3)patrolMoves[i];
            Gizmos.DrawLine(point, next);
            Gizmos.DrawSphere(next, 0.08f);
            point = next;
        }
    }
}
