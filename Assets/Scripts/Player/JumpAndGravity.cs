using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class JumpAndGravity : MonoBehaviour
{
    [Header("跳跃速度设置")]
    [SerializeField] float jumpVelNormal = 10f;   // 普通重力下的跳跃速度
    [SerializeField] float jumpVelLow = 12f;      // 低重力下的跳跃速度
    [SerializeField] float jetPackForce = 3f;     // 零重力时喷气背包的推力（按住跳跃键持续上升）

    [Header("重力倍率")]
    [SerializeField] float normalG = 1f;          // 普通重力倍率
    [SerializeField] float lowG = 0.5f;           // 低重力倍率
    [SerializeField] float zeroG = 0f;            // 零重力倍率（漂浮）

    [Header("漂浮持续时间（秒）")]
    [SerializeField] float floatDuration = 4f;

    [Header("按键绑定")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;     // 跳跃键
    [SerializeField] KeyCode lowGKey = KeyCode.Alpha2;    // 手动切换低重力
    [SerializeField] KeyCode zeroGKey = KeyCode.Alpha3;   // 手动切换零重力

    Rigidbody2D rb;
    float currentGScale;     // 当前重力倍率
    float floatTimer;        // 漂浮计时器
    bool isFloating;         // 是否处于漂浮状态

    [Header("落地检测参数")]
    [SerializeField] LayerMask groundLayer;       // 地面层
    [SerializeField] Transform groundCheck;       // 检测落地的圆心位置
    [SerializeField] float checkRadius = 0.2f;    // 检测半径

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentGScale = normalG;
        rb.gravityScale = normalG; // 使用正常重力作为默认值
    }

    void Update()
    {
        // 手动切换重力模式
        if (Input.GetKeyDown(lowGKey)) SetGravity(lowG);
        if (Input.GetKeyDown(zeroGKey)) SetGravity(zeroG);

        // 跳跃输入
        if (Input.GetKeyDown(jumpKey))
        {
            if (IsGrounded())
            {
                // 根据当前重力模式设置不同的跳跃初速度
                if (Mathf.Approximately(currentGScale, normalG))
                    rb.velocity = new Vector2(rb.velocity.x, jumpVelNormal);
                else if (Mathf.Approximately(currentGScale, lowG))
                    rb.velocity = new Vector2(rb.velocity.x, jumpVelLow);
                else // zeroG 模式
                {
                    // 在零重力下，从地面起跳时仍给一个较小初速度
                    rb.velocity = new Vector2(rb.velocity.x, jumpVelLow * 0.6f);
                }
            }
            else
            {
                // 空中按住跳跃键时，在零重力下给予微小上升力（喷气效果）
                if (Mathf.Approximately(currentGScale, zeroG))
                    rb.AddForce(Vector2.up * jetPackForce, ForceMode2D.Impulse);
            }
        }

        // 调试信息
        Debug.Log($"是否在地面: {IsGrounded()}, 重力倍率: {currentGScale}, 检测位置: {groundCheck.position}");
    }

    void FixedUpdate()
    {
        // 漂浮计时逻辑
        if (isFloating)
        {
            floatTimer -= Time.fixedDeltaTime;
            if (floatTimer <= 0)
                SetGravity(lowG); // 漂浮时间结束后恢复低重力
        }

        // 直接设置重力倍率，不使用复杂的物理计算
        if (!Mathf.Approximately(currentGScale, rb.gravityScale))
        {
            rb.gravityScale = currentGScale;
        }
    }

    // 检测角色是否接触地面
    bool IsGrounded()
    {
        Collider2D collider = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        bool grounded = collider != null;
        
        // 调试地面检测
        if (grounded)
            Debug.Log($"检测到地面！碰撞体: {collider.name}, 位置: {groundCheck.position}");
        else
            Debug.Log($"未检测到地面，检测位置: {groundCheck.position}, 半径: {checkRadius}, 图层: {groundLayer.value}");
        
        return grounded;
    }

    // 设置当前重力倍率
    void SetGravity(float scale)
    {
        currentGScale = scale;
        rb.gravityScale = scale;

        if (Mathf.Approximately(scale, zeroG))
        {
            // 进入零重力漂浮状态
            isFloating = true;
            floatTimer = floatDuration;
            // 在零重力时给一个很小的向下的力，避免完全漂浮
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Min(rb.velocity.y, -0.1f));
        }
        else
        {
            isFloating = false;
        }
    }

    // Scene 视图中绘制检测范围辅助圈
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }
}