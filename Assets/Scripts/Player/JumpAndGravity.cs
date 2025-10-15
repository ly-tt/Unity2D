using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class JumpAndGravity : MonoBehaviour
{
    [Header("跳跃速度")]
    [SerializeField] float jumpVelNormal = 10f;   // 普通档
    [SerializeField] float jumpVelLow = 12f;   // 低重力档
    [SerializeField] float jetPackForce = 3f;    // 零重力时的“喷气”推力（每按一次给一下）

    [Header("重力档位")]
    [SerializeField] float normalG = 1f;
    [SerializeField] float lowG = 0.5f;
    [SerializeField] float zeroG = 0f;

    [Header("漂浮时长")]
    [SerializeField] float floatDuration = 4f;

    [Header("按键绑定")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode lowGKey = KeyCode.Alpha2;   // 手动切低重力
    [SerializeField] KeyCode zeroGKey = KeyCode.Alpha3;   // 手动切漂浮

    Rigidbody2D rb;
    float currentGScale;     // 当前重力倍数
    float floatTimer;
    bool isFloating;

    // 地面检测（用 LayerMask，更稳）
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheck;
    [SerializeField] float checkRadius = 0.2f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentGScale = normalG;
        rb.gravityScale = 0;   // 我们全部手动加力，所以先把内置重力关掉
    }

    void Update()
    {
        // 手动切档
        if (Input.GetKeyDown(lowGKey)) SetGravity(lowG);
        if (Input.GetKeyDown(zeroGKey)) SetGravity(zeroG);

        // 跳跃
        if (Input.GetKeyDown(jumpKey))
        {
            if (IsGrounded())
            {
                // 根据当前重力档给不同的跳跃速度
                if (Mathf.Approximately(currentGScale, normalG))
                    rb.velocity = new Vector2(rb.velocity.x, jumpVelNormal);
                else if (Mathf.Approximately(currentGScale, lowG))
                    rb.velocity = new Vector2(rb.velocity.x, jumpVelLow);
                else // zeroG
                {
                    // 零重力下第一次起跳也给一点点向上速度，方便离地
                    rb.velocity = new Vector2(rb.velocity.x, jumpVelLow * 0.6f);
                }
            }
            else
            {
                // 空中按跳跃：零重力时给“喷气”微调，其他档什么都不做（你也可以二段跳）
                if (Mathf.Approximately(currentGScale, zeroG))
                    rb.AddForce(Vector2.up * jetPackForce, ForceMode2D.Impulse);
            }
        }
    }

    void FixedUpdate()
    {
        // 漂浮计时
        if (isFloating)
        {
            floatTimer -= Time.fixedDeltaTime;
            if (floatTimer <= 0) SetGravity(lowG);
        }

        // 手动补重力
        float missingG = Physics2D.gravity.y * (currentGScale - rb.gravityScale);
        rb.AddForce(Vector2.up * missingG * rb.mass, ForceMode2D.Force);
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    void SetGravity(float scale)
    {
        currentGScale = scale;
        rb.gravityScale = 0;
        if (Mathf.Approximately(scale, zeroG))
        {
            isFloating = true;
            floatTimer = floatDuration;
        }
        else
        {
            isFloating = false;
        }
    }

    // 小工具：在 Scene 视图能看到地面检测范围
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }
}