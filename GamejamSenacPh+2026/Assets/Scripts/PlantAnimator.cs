using UnityEngine;
using UnityEngine.EventSystems;

public class PlantAnimator : MonoBehaviour
{

    [SerializeField] Transform visual;

  
    [SerializeField] float idleSwayDegrees = 3f;
    [SerializeField] float idleSwaySpeed   = 0.7f;
    [SerializeField] float idleScaleAmount = 0.025f;
    [SerializeField] float idleScaleSpeed  = 0.5f;

   
    [SerializeField] float springStiffness  = 180f;  
    [SerializeField] float springDamping    = 8f;   
    [SerializeField] float springScaleMult  = 0.012f; 

   
    [SerializeField] float hoverImpulse = 25f;

   
    [SerializeField] bool rotateFromBase = true;

       float _rotPhase;
    float _scalePhase;
    float _rotSpeed;
    float _scaleSpeed;
    int   _swayDir;

    float _springAngle;  
    float _springVelocity;

       Vector3 _baseScale;
    Vector3 _baseLocalPos;

    void Awake()
    {
        if (visual == null) visual = transform;
        _baseScale    = visual.localScale;
        _baseLocalPos = visual.localPosition;

        _rotPhase   = Random.Range(0f, Mathf.PI * 2f);
        _scalePhase = Random.Range(0f, Mathf.PI * 2f);
        _rotSpeed   = idleSwaySpeed   * Random.Range(0.8f, 1.2f);
        _scaleSpeed = idleScaleSpeed  * Random.Range(0.8f, 1.2f);
        _swayDir    = Random.value > 0.5f ? 1 : -1;
    }

    void Update()
    {
        float dt = Time.deltaTime;

        _rotPhase   += _rotSpeed   * dt * Mathf.PI * 2f;
        _scalePhase += _scaleSpeed * dt * Mathf.PI * 2f;

        float springForce = -springStiffness * _springAngle
                            - springDamping  * _springVelocity;
        _springVelocity += springForce * dt;
        _springAngle    += _springVelocity * dt;

        float idleAngle  = Mathf.Sin(_rotPhase) * idleSwayDegrees * _swayDir;
        float totalAngle = idleAngle + _springAngle;

         visual.localEulerAngles = new Vector3(0f, 0f, totalAngle);

         if (rotateFromBase)
        {
            float rad     = totalAngle * Mathf.Deg2Rad;
            float height  = _baseScale.y * 0.5f;
            visual.localPosition = new Vector3(
                _baseLocalPos.x + Mathf.Sin(rad) * height,
                _baseLocalPos.y + (Mathf.Cos(rad) - 1f) * height,
                _baseLocalPos.z);
        }

        float breathe   = 1f + Mathf.Sin(_scalePhase) * idleScaleAmount;
        float springLean = Mathf.Abs(_springAngle) * springScaleMult;
        visual.localScale = new Vector3(
            _baseScale.x * (breathe - springLean),
            _baseScale.y * (breathe + springLean),
            _baseScale.z);
    }

   
    public void AddImpulse(float degrees)
    {
        _springVelocity += degrees;
    }

    
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
          
            float dir = _springAngle + Mathf.Sin(_rotPhase) * idleSwayDegrees * _swayDir >= 0f ? -1f : 1f;
            
            AddImpulse(hoverImpulse * -dir);
        }
    
    
    }

    public void OnPointerExit(PointerEventData _) { }

      public void ReactToImpact(float force = 1f)
    {
        float dir = Random.value > 0.5f ? 1f : -1f;
        AddImpulse(hoverImpulse * force * dir);
    }
}