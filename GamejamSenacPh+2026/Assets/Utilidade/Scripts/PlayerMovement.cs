using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    public float moveSpeed = 8f;

    public float airAcceleration = 40f;
    public float jumpForce = 14f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.15f;

    float coyoteTimeCounter;
    float jumpBufferCounter;

    Rigidbody2D rb;
    bool isGrounded;
    float horizontalInput;
    [HideInInspector] public bool isSwinging = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

         if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

       
        if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }

      
        if (Input.GetKeyUp(KeyCode.Space) && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        FlipSprite();
    }

    void FixedUpdate()
    {
        
        if (isSwinging) return;

        if (isGrounded)
        {
           
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        }
        else if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            
            float targetSpeed = horizontalInput * moveSpeed;
            float newVx = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, airAcceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(newVx, rb.linearVelocity.y);
        }
       
    }

    void FlipSprite()
    {
        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}