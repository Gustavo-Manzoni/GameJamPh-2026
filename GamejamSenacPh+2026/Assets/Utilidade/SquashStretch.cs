using UnityEngine;

public class SquashStretch : MonoBehaviour
{
    [SerializeField] private Transform visual;

    [SerializeField] private float stiffness = 250f;
    [SerializeField] private float damping = 16f;

    [SerializeField] private bool reactToVelocity = false;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private float velocityInfluence = 0.02f;
    [SerializeField] private float maxStretch = 0.35f;

    private SpringVector2 scaleSpring;
    private Vector3 baseScale;

    private void Awake()
    {
        if (visual == null) visual = transform;
        baseScale = visual.localScale;
        scaleSpring = new SpringVector2(Vector2.one) { stiffness = stiffness, damping = damping };
    }

    private void Update()
    {
        Vector2 target = Vector2.one;

        if (reactToVelocity && body != null)
        {
            float speed = body.linearVelocity.magnitude;
            float stretch = Mathf.Clamp(speed * velocityInfluence, 0f, maxStretch);
            target = new Vector2(1f + stretch, 1f - stretch * 0.6f);
        }

        Vector2 s = scaleSpring.Update(target, Time.deltaTime);
        visual.localScale = new Vector3(baseScale.x * s.x, baseScale.y * s.y, baseScale.z);
    }

    public void Kick(Vector2 velocityKick)
    {
        scaleSpring.velocity += velocityKick;
    }

    public void SnapTo(Vector2 scale)
    {
        scaleSpring.SetInstant(scale);
    }
}