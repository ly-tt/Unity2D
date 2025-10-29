using UnityEngine;

/// <summary>
/// 挂在所有 Tag="Box" 的箱子上，控制初始重力方向。
/// </summary>
public class BoxGravity : MonoBehaviour
{
    [Tooltip("如果箱子初始在天花板（重力向上），请勾选此项")]
    public bool startsInverted = false;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Box must have a Rigidbody2D!", this);
            return;
        }

        // 设置初始重力方向
        rb.gravityScale = startsInverted ? -1f : 1f;

        // 防止旋转（保持 upright）
        rb.freezeRotation = true;
    }
}