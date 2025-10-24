using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MoveJump : MonoBehaviour
{
    [Header("移动参数")]
    public float walkSpeed = 2f;
    public float runMultiplier = 10f;

    [Header("跳跃参数")]
    [SerializeField] float jumpVelNormal = 10f;
    [SerializeField] float jumpVelLow = 12f;
    [SerializeField] float jetPackForce = 3f;

    [Header("重力倍率")]
    [SerializeField] float normalG = 1f;
    [SerializeField] float lowG = 0.5f;
    [SerializeField] float zeroG = 0f;

    [Header("漂浮持续时间（秒）")]
    [SerializeField] float floatDuration = 4f;

    [Header("地面检测参数")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float checkRadius = 0.2f;
    [SerializeField] LayerMask groundLayer;

    [Header("按键绑定")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode lowGKey = KeyCode.Alpha2;
    [SerializeField] KeyCode zeroGKey = KeyCode.Alpha3;

    private Rigidbody2D rb;
    private Collider2D playerCollider; // 新增：玩家碰撞体引用
    private float moveInput;
    private bool isRunning;
    private bool isGrounded;
    private bool isFloating;
    private float floatTimer;
    private float currentGScale;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>(); // 获取碰撞体
        currentGScale = normalG;
        rb.gravityScale = normalG;
    }

    void Update()
    {
        // --- 输入处理 ---
        moveInput = Input.GetAxisRaw("Horizontal");
        isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // --- 落地检测 ---
        isGrounded = GroundCheck();

        // --- 朝向翻转 ---
        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        // --- 重力切换 ---
        if (Input.GetKeyDown(lowGKey)) SetGravity(lowG);
        if (Input.GetKeyDown(zeroGKey)) SetGravity(zeroG);

        // --- 跳跃逻辑 ---
        HandleJumpAndGravity();

        // 调试信息
        Debug.Log($"Grounded: {isGrounded}, GScale: {currentGScale}, Vel: {rb.velocity}");
    }

    void FixedUpdate()
    {
        Move();
        UpdateFloatTimer();

        // 关键修复：在地面上时强制停止下落
        if (isGrounded && rb.velocity.y < 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            
            // 额外保险：如果玩家还在下沉，稍微向上调整位置
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);
            if (hit.collider != null && hit.distance < 0.1f)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.05f, transform.position.z);
            }
        }

        if (!Mathf.Approximately(currentGScale, rb.gravityScale))
            rb.gravityScale = currentGScale;
    }

    // --- 移动控制 ---
    void Move()
    {
        float speed = walkSpeed * (isRunning ? runMultiplier : 1f);
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
    }

    // --- 跳跃与喷气 ---
    void HandleJumpAndGravity()
    {
        if (Input.GetKeyDown(jumpKey))
        {
            if (isGrounded)
            {
                if (Mathf.Approximately(currentGScale, normalG))
                    rb.velocity = new Vector2(rb.velocity.x, jumpVelNormal);
                else if (Mathf.Approximately(currentGScale, lowG))
                    rb.velocity = new Vector2(rb.velocity.x, jumpVelLow);
                else // zeroG 模式
                    rb.velocity = new Vector2(rb.velocity.x, jumpVelLow * 0.6f);
            }
            else if (Mathf.Approximately(currentGScale, zeroG))
            {
                // 空中喷气
                rb.AddForce(Vector2.up * jetPackForce, ForceMode2D.Impulse);
            }
        }
    }

    // --- 落地检测 ---
    bool GroundCheck()
    {
        Collider2D hit = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        return hit != null;
    }

    // --- 漂浮计时 ---
    void UpdateFloatTimer()
    {
        if (isFloating)
        {
            floatTimer -= Time.fixedDeltaTime;
            if (floatTimer <= 0)
                SetGravity(lowG);
        }
    }

    // --- 重力切换 ---
    void SetGravity(float scale)
    {
        currentGScale = scale;
        rb.gravityScale = scale;

        if (Mathf.Approximately(scale, zeroG))
        {
            isFloating = true;
            floatTimer = floatDuration;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Min(rb.velocity.y, -0.1f));
        }
        else
        {
            isFloating = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }
}