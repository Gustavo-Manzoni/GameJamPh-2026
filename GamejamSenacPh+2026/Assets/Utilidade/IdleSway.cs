using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SwayLimb
{
    public Transform pivot;
    public float swayAngle = 5f;
    public float speedMultiplier = 1f;
    public float phaseOffset = 0f;
    public float stiffness = 90f;
    public float damping = 8f;

    [System.NonSerialized] public Quaternion baseRotation;
    [System.NonSerialized] public SpringFloat spring;
}

// generic "gostosinho" idle breathing sway, drop on anything with a root pivot and optional limb pivots (like the menu fighter idle)
public class IdleSway : MonoBehaviour
{
    [SerializeField] private Transform rootPivot;
    [SerializeField] private float breathSpeed = 1.5f;
    [SerializeField] private float rootBobAmount = 0.035f;
    [SerializeField] private float rootStretchAmount = 0.02f;
    [SerializeField] private float rootLeanAngle = 1.5f;
    [SerializeField] private float rootStiffness = 70f;
    [SerializeField] private float rootDamping = 8f;

    [SerializeField] private List<SwayLimb> limbs = new List<SwayLimb>();

    private Vector3 rootBasePosition;
    private Vector3 rootBaseScale;
    private Quaternion rootBaseRotation;
    private SpringFloat rootLeanSpring;
    private float phase;

    private void Awake()
    {
        if (rootPivot == null) rootPivot = transform;

        rootBasePosition = rootPivot.localPosition;
        rootBaseRotation = rootPivot.localRotation;
        rootBaseScale = rootPivot.localScale;
        rootLeanSpring = new SpringFloat(0f) { stiffness = rootStiffness, damping = rootDamping };
        phase = Random.Range(0f, Mathf.PI * 2f);

        foreach (var limb in limbs)
        {
            if (limb.pivot == null) continue;
            limb.baseRotation = limb.pivot.localRotation;
            limb.spring = new SpringFloat(0f) { stiffness = limb.stiffness, damping = limb.damping };
        }
    }

    private void Update()
    {
        phase += Time.deltaTime * breathSpeed;
        float breath = Mathf.Sin(phase);

        float lean = rootLeanSpring.Update(Mathf.Sin(phase * 0.52f) * rootLeanAngle, Time.deltaTime);
        rootPivot.localPosition = rootBasePosition + Vector3.up * (breath * rootBobAmount);
        rootPivot.localRotation = rootBaseRotation * Quaternion.Euler(0f, 0f, lean);
        rootPivot.localScale = new Vector3(
            rootBaseScale.x * (1f - breath * rootStretchAmount),
            rootBaseScale.y * (1f + breath * rootStretchAmount),
            rootBaseScale.z);

        foreach (var limb in limbs)
        {
            if (limb.pivot == null || limb.spring == null) continue;

            float target = Mathf.Sin(phase * limb.speedMultiplier + limb.phaseOffset) * limb.swayAngle;
            float angle = limb.spring.Update(target, Time.deltaTime);
            limb.pivot.localRotation = limb.baseRotation * Quaternion.Euler(0f, 0f, angle);
        }
    }

    // extra punch on top of the idle sway, e.g. call when hit/interacted with
    public void Kick(float direction = 1f, float force = 30f)
    {
        float side = Mathf.Sign(direction == 0f ? 1f : direction);
        rootLeanSpring.velocity += side * force * 0.4f;

        foreach (var limb in limbs)
        {
            if (limb.spring != null)
                limb.spring.velocity += side * force;
        }
    }
}
