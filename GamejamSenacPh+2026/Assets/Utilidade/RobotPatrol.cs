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

    
    [SerializeField] private Transform visual;
    [SerializeField] private SquashStretch squashStretch;
    [SerializeField] private float bobAmplitude = 0.045f;
    [SerializeField] private float bobFrequency = 9f;
    [SerializeField] private float leanAngle = 10f;
    [SerializeField] private float visualResponsiveness = 14f;
    [SerializeField, Range(0f, 1f)] private float hitCameraTrauma = 0.08f;

    private Rigidbody2D body;
    private Vector2 routeOrigin;
    private Vector2 waypoint;
    private Vector2 velocity;
    private Vector3 visualBasePosition;
    private int moveIndex;
    private float pauseTimer;
    private float bobTimer;
    private float facingY;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = true;
        body.bodyType = RigidbodyType2D.Kinematic;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        routeOrigin = body.position;
            visualBasePosition = visual.localPosition;

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
        if (visual == null) return;

        float speed01 = velocity.magnitude / moveSpeed;
        if (speed01 > 0.01f)
            bobTimer += Time.deltaTime * bobFrequency * speed01;

        if (Mathf.Abs(velocity.x) > 0.01f)
            facingY = velocity.x > 0f ? 0f : 180f;

        float bob = Mathf.Abs(Mathf.Sin(bobTimer)) * bobAmplitude * speed01;
        float lean = -velocity.x / moveSpeed * leanAngle;
        visual.localPosition = Vector3.Lerp(
            visual.localPosition,
            visualBasePosition + Vector3.up * bob,
            Time.deltaTime * visualResponsiveness);
        visual.localRotation = Quaternion.Lerp(
            visual.localRotation,
            Quaternion.Euler(0f, facingY, lean),
            Time.deltaTime * visualResponsiveness);
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
