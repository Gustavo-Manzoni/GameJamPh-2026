using UnityEngine;

public class MenuFighterIdle : MonoBehaviour
{
   
    [SerializeField] Transform rootPivot;
    [SerializeField] Transform torsoPivot;
    [SerializeField] Transform leftArmPivot;
    [SerializeField] Transform rightArmPivot;

 
    [SerializeField] float breathSpeed = 1.5f;
    [SerializeField] float rootBobAmount = 0.035f;
    [SerializeField] float rootLeanAngle = 1.5f;
    [SerializeField] float torsoSwayAngle = 3f;
    [SerializeField] float torsoStretch = 0.025f;

  
    [SerializeField] float leftArmSwayAngle = 4.5f;
    [SerializeField] float rightArmSwayAngle = 3.5f;
    [SerializeField] float armLeadLag = 0.55f;
    [SerializeField] float guardPulseAngle = 2f;

   
    [SerializeField] float rootStiffness = 70f;
    [SerializeField] float rootDamping = 8f;
    [SerializeField] float torsoStiffness = 95f;
    [SerializeField] float torsoDamping = 9f;
    [SerializeField] float armStiffness = 60f;
    [SerializeField] float armDamping = 6f;

    Vector3 rootBasePosition;
    Vector3 rootBaseScale;
    Vector3 torsoBaseScale;
    Quaternion rootBaseRotation;
    Quaternion torsoBaseRotation;
    Quaternion leftArmBaseRotation;
    Quaternion rightArmBaseRotation;
    SpringFloat rootLeanSpring;
    SpringFloat torsoSpring;
    SpringFloat leftArmSpring;
    SpringFloat rightArmSpring;

    void Awake()
    {
        if (rootPivot == null) rootPivot = transform;
        CacheBasePose();

        rootLeanSpring = new SpringFloat(0f) { stiffness = rootStiffness, damping = rootDamping };
        torsoSpring = new SpringFloat(0f) { stiffness = torsoStiffness, damping = torsoDamping };
        leftArmSpring = new SpringFloat(0f) { stiffness = armStiffness, damping = armDamping };
        rightArmSpring = new SpringFloat(0f) { stiffness = armStiffness, damping = armDamping };
    }

    void LateUpdate()
    {
        float time = Time.unscaledTime;
        float dt = Time.unscaledDeltaTime;
        float phase = time * breathSpeed + GetInstanceID() * 0.071f;

        AnimateRoot(phase, dt);
        AnimateTorso(phase, dt);
        AnimateArms(phase, dt);
    }

    void CacheBasePose()
    {
        rootBasePosition = rootPivot.localPosition;
        rootBaseRotation = rootPivot.localRotation;
        rootBaseScale = rootPivot.localScale;

        if (torsoPivot != null)
        {
            torsoBaseRotation = torsoPivot.localRotation;
            torsoBaseScale = torsoPivot.localScale;
        }
        if (leftArmPivot != null) leftArmBaseRotation = leftArmPivot.localRotation;
        if (rightArmPivot != null) rightArmBaseRotation = rightArmPivot.localRotation;
    }

    void AnimateRoot(float phase, float dt)
    {
        float inhale = Mathf.Sin(phase);
        float lean = rootLeanSpring.Update(Mathf.Sin(phase * 0.52f) * rootLeanAngle, dt);
        rootPivot.localPosition = rootBasePosition + Vector3.up * (inhale * rootBobAmount);
        rootPivot.localRotation = rootBaseRotation * Quaternion.Euler(0f, 0f, lean);
        rootPivot.localScale = new Vector3(
            rootBaseScale.x * (1f - inhale * torsoStretch * 0.35f),
            rootBaseScale.y * (1f + inhale * torsoStretch * 0.35f),
            rootBaseScale.z);
    }

    void AnimateTorso(float phase, float dt)
    {
        float torsoAngle = torsoSpring.Update(Mathf.Sin(phase * 0.77f + 0.35f) * torsoSwayAngle, dt);
        float chestBreath = Mathf.Sin(phase) * torsoStretch;
        torsoPivot.localRotation = torsoBaseRotation * Quaternion.Euler(0f, 0f, torsoAngle);
        torsoPivot.localScale = new Vector3(
            torsoBaseScale.x * (1f - chestBreath),
            torsoBaseScale.y * (1f + chestBreath),
            torsoBaseScale.z);
    }

    void AnimateArms(float phase, float dt)
    {
      
     float leftTarget = Mathf.Sin(phase * 0.92f + armLeadLag) * leftArmSwayAngle
            + Mathf.Sin(phase * 1.91f) * guardPulseAngle;
        float rightTarget = Mathf.Sin(phase * 0.74f - armLeadLag) * rightArmSwayAngle
            - Mathf.Sin(phase * 1.61f + 0.8f) * guardPulseAngle;

        float leftAngle = leftArmSpring.Update(leftTarget, dt);
        float rightAngle = rightArmSpring.Update(rightTarget, dt);

        if (leftArmPivot != null)
            leftArmPivot.localRotation = leftArmBaseRotation * Quaternion.Euler(0f, 0f, leftAngle);
        if (rightArmPivot != null)
            rightArmPivot.localRotation = rightArmBaseRotation * Quaternion.Euler(0f, 0f, rightAngle);
    }
    public void KickGuard(float direction = 1f)
    {
        float side = Mathf.Sign(direction == 0f ? 1f : direction);
        torsoSpring.velocity += side * 22f;
        leftArmSpring.velocity += side * 45f;
        rightArmSpring.velocity += side * 32f;
        rootLeanSpring.velocity += side * 12f;
    }
}
