using UnityEngine;

public class TopDownCharacterController : MonoBehaviour
{
    [SerializeField] float moveSpeed    = 6f;
    [SerializeField] float acceleration = 12f;
    [SerializeField] float deceleration = 18f;

  
    [SerializeField] float walkSquashSpeed   = 8f;
    [SerializeField] float walkSquashAmount  = 0.12f;
    [SerializeField] float walkRotDegrees    = 5f;
    [SerializeField] float walkYAmount       = 0.06f;

    [SerializeField] float rotationSpeed;

   
    [SerializeField] float idleSquashSpeed  = 1.5f;
    [SerializeField] float idleSquashAmount = 0.03f;

   
    [SerializeField] float animTransitionSpeed = 8f;

   
    [SerializeField] SpriteRenderer spriteRenderer ;

       Rigidbody2D _rb;
    Vector2     _velocity;
    bool        _canMove;
    int         _facingDir = 1;

    float _animPhase;
    float _squashOffset;
    Vector3 _baseScale;

    float _curSquashSpeed;
    float _curSquashAmount;
    float _curRotDegrees;
    float _curYAmount;

    public void SetCanMove(bool canMove) => _canMove = canMove;

 
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale   = 0f;
        _rb.freezeRotation = true;
        ServiceLocator.Register(this);
        _squashOffset = Random.Range(0f, Mathf.PI * 2f);
        _canMove      = true;
    }

    void Start()
    {

        _baseScale = spriteRenderer.transform.localScale;

        _curSquashSpeed  = idleSquashSpeed;
        _curSquashAmount = idleSquashAmount;
        _curRotDegrees   = 0f;
        _curYAmount      = 0f;
    }

    void Update()
    {
        LerpAnimParams(Time.deltaTime);
        TickAnimPhase(Time.deltaTime);
        ApplySquash();
        ApplyWalkRotation();
        ApplyYPos();
    }

    void FixedUpdate()
    {
        if (!_canMove)
        {
            _velocity = Vector2.MoveTowards(_velocity, Vector2.zero,
                                            deceleration * Time.fixedDeltaTime);
            _rb.linearVelocity = _velocity;
            return;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"),
                                     Input.GetAxisRaw("Vertical")).normalized;

        float accel = input.sqrMagnitude > 0f ? acceleration : deceleration;
        _velocity = Vector2.MoveTowards(_velocity, input * moveSpeed,
                                        accel * Time.fixedDeltaTime);
        _rb.linearVelocity = _velocity;

        if (input.x != 0f)
            _facingDir = input.x > 0f ? 1 : -1;
    }

   
    bool IsWalking => _velocity.sqrMagnitude > 0.01f;

    void LerpAnimParams(float dt)
    {
        float t       = animTransitionSpeed * dt;
        bool  walking = IsWalking;

        float tgtSquashSpeed  = walking ? walkSquashSpeed  : idleSquashSpeed;
        float tgtSquashAmount = walking ? walkSquashAmount : idleSquashAmount;
        float tgtRotDegrees   = walking ? walkRotDegrees   : 0f;
        float tgtYAmount      = walking ? walkYAmount      : 0f;

        _curSquashSpeed  = Mathf.Lerp(_curSquashSpeed,  tgtSquashSpeed,  t);
        _curSquashAmount = Mathf.Lerp(_curSquashAmount, tgtSquashAmount, t);
        _curRotDegrees   = Mathf.Lerp(_curRotDegrees,   tgtRotDegrees,   t);
        _curYAmount      = Mathf.Lerp(_curYAmount,      tgtYAmount,      t);
    }

    void TickAnimPhase(float dt) => _animPhase += _curSquashSpeed * dt;

    void ApplySquash()
    {
        float s  = Mathf.Sin(_animPhase + _squashOffset);
        float sx = _baseScale.x * (1f - s * _curSquashAmount);
        float sy = _baseScale.y * (1f + s * _curSquashAmount);
        spriteRenderer.transform.localScale = new Vector3(sx, sy, _baseScale.z);
    }

    Quaternion _flipRotation = Quaternion.identity; 

void ApplyWalkRotation()
{
    float leanAngle = Mathf.Sin(_animPhase + _squashOffset) * _curRotDegrees;
    float facingRotationAngle = _facingDir == 1 ? 0f : 180f;

  
    Quaternion targetFlip = Quaternion.Euler(0f, facingRotationAngle, 0f);
    _flipRotation = Quaternion.Lerp(_flipRotation, targetFlip, rotationSpeed * Time.deltaTime);

   
    spriteRenderer.transform.localRotation = _flipRotation * Quaternion.Euler(0f, 0f, leanAngle);
}

    void ApplyYPos()
    {
        float yOff = Mathf.Sin(_animPhase + _squashOffset) * _curYAmount;
        Vector3 lp = spriteRenderer.transform.localPosition;
        spriteRenderer.transform.localPosition = new Vector3(lp.x, yOff, lp.z);
    }
}
