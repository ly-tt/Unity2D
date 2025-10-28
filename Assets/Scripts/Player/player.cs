using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Header("Player Move")]
    public Rigidbody2D rb;
    private float xInput;
    public float moveSpeed = 5;
    public Animator playerAnimator;
    private int facingDri = 1;
    [HideInInspector] public bool isRight = true;
    [HideInInspector] public bool isMoving;

    [Header("Player Jump")]
    public float jumpForce = 5;
    public LayerMask groundLayer; // ← 应包含 "Ground" 和 "Box" 层！
    [SerializeField] private float ground_check_distance;
    [SerializeField] private Vector2 groundCheckOffset = Vector2.zero;
    [HideInInspector] public bool isGround;
    [HideInInspector] public bool isJump = false;
    [HideInInspector] public bool isAir = false;

    [Header("Player Dash")]
    public float dashSpeed = 10f;
    public float dashTime = 0.2f;
    public float dashCoolDownTime = 1f;
    [HideInInspector] public bool isDashing = false;
    [HideInInspector] public bool canDash = true;

    [Header("Player Dead")]
    public bool isDead = false;

    // ===== 重力颠倒系统 =====
    [Header("Gravity Invert")]
    public bool isGravityInverted = false;
    private Vector2 groundCheckDirection => isGravityInverted ? Vector2.up : Vector2.down;
    private KeyCode jumpKey => isGravityInverted ? KeyCode.S : KeyCode.W;

    // ===== 星星碎片能力标志 =====
    [HideInInspector] public bool hasGravityInvertAbility = false;     // 碎片1：玩家重力反转
    [HideInInspector] public bool hasBoxGravityInvertAbility = false;  // 碎片2：箱子重力反转

    // ===== 推拉箱子系统 =====
    [Header("Box Interaction")]
    public LayerMask boxLayer; // ← 必须设为 "Box" 层！
    public float boxCheckDistance = 0.6f;
    [SerializeField] private Vector2 boxCheckOffset = new Vector2(0f, -0.3f);
    [HideInInspector] public bool isPulling = false;

    private Rigidbody2D currentBox = null;

    void Update()
    {
        FlipController();
        PlayerMove();
        check_ground();
        PlayerJump();
        HandleGravityInvert();
        HandleBoxInteraction();
        HandleBoxGravityInvert();
    }

    void FixedUpdate()
    {
        PlayerMoveFix();
    }

    private void check_ground()
    {
        Vector2 rayOrigin = (Vector2)transform.position + groundCheckOffset;
        isGround = Physics2D.Raycast(rayOrigin, groundCheckDirection, ground_check_distance, groundLayer);
    }

    private void FlipController()
    {
        if (isPulling) return;

        if (rb.velocity.x > 0 && !isRight)
            Flip();
        else if (rb.velocity.x < 0 && isRight)
            Flip();
    }

    private void Flip()
    {
        facingDri *= -1;
        isRight = !isRight;
        transform.Rotate(0, 180, 0);
    }

    private void PlayerMove()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        isMoving = rb.velocity.x != 0;
        playerAnimator.SetBool("isMoving", isMoving);
    }

    private void PlayerMoveFix()
    {
        rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
    }

    private void PlayerJump()
    {
        playerAnimator.SetBool("isGround", isGround);
        playerAnimator.SetFloat("yVelocity", rb.velocity.y);

        if (Input.GetKeyDown(jumpKey) && isGround)
        {
            float jumpVelocity = isGravityInverted ? -jumpForce : jumpForce;
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
        }
    }

    private void HandleGravityInvert()
    {
        // 只有获得碎片1后，X 键才生效
        if (hasGravityInvertAbility && Input.GetKeyDown(KeyCode.X))
        {
            transform.Rotate(180, 0, 0);
            rb.gravityScale *= -1;
            isGravityInverted = !isGravityInverted;
        }
    }

    private void HandleBoxInteraction()
    {
        isPulling = false;
        currentBox = null;

        bool isHoldingZ = Input.GetKey(KeyCode.Z);
        bool wantsToMoveLeft = xInput < 0;
        bool wantsToMoveRight = xInput > 0;

        Vector2 rayOrigin = (Vector2)transform.position + boxCheckOffset;

        RaycastHit2D hitLeft = Physics2D.Raycast(rayOrigin, Vector2.left, boxCheckDistance, boxLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(rayOrigin, Vector2.right, boxCheckDistance, boxLayer);

        Debug.DrawRay(rayOrigin, Vector2.left * boxCheckDistance, Color.cyan);
        Debug.DrawRay(rayOrigin, Vector2.right * boxCheckDistance, Color.magenta);

        if (hitLeft.collider != null)
        {
            currentBox = hitLeft.collider.attachedRigidbody;

            if (wantsToMoveRight)
            {
                if (currentBox != null)
                    currentBox.velocity = new Vector2(moveSpeed, currentBox.velocity.y);
            }
            else if (isHoldingZ && wantsToMoveLeft)
            {
                isPulling = true;
                if (currentBox != null)
                    currentBox.velocity = new Vector2(-moveSpeed, currentBox.velocity.y);
            }
        }
        else if (hitRight.collider != null)
        {
            currentBox = hitRight.collider.attachedRigidbody;

            if (wantsToMoveLeft)
            {
                if (currentBox != null)
                    currentBox.velocity = new Vector2(-moveSpeed, currentBox.velocity.y);
            }
            else if (isHoldingZ && wantsToMoveRight)
            {
                isPulling = true;
                if (currentBox != null)
                    currentBox.velocity = new Vector2(moveSpeed, currentBox.velocity.y);
            }
        }
    }

    private void HandleBoxGravityInvert()
    {
        // 只有获得碎片2后，C 键才生效
        if (hasBoxGravityInvertAbility && Input.GetKeyDown(KeyCode.C))
        {
            GameObject[] boxes = GameObject.FindGameObjectsWithTag("Box");

            foreach (GameObject box in boxes)
            {
                Rigidbody2D rbBox = box.GetComponent<Rigidbody2D>();
                if (rbBox != null)
                {
                    rbBox.gravityScale *= -1;
                    rbBox.velocity = new Vector2(rbBox.velocity.x, -rbBox.velocity.y);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Vector2 groundOrigin = (Vector2)transform.position + groundCheckOffset;
        Vector2 groundEnd = groundOrigin + groundCheckDirection * ground_check_distance;
        Gizmos.color = isGround ? Color.green : Color.red;
        Gizmos.DrawLine(groundOrigin, groundEnd);

        Vector2 boxOrigin = (Vector2)transform.position + boxCheckOffset;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(boxOrigin, boxOrigin + Vector2.left * boxCheckDistance);
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(boxOrigin, boxOrigin + Vector2.right * boxCheckDistance);
    }
}