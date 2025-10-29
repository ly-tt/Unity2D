using UnityEngine;

public class AutoFollowBox : MonoBehaviour
{
    public LayerMask boxLayer;
    public float detectionRadius = 0.7f;
    public Vector2 boxCheckOffset = new Vector2(0f, -0.2f); // 检测高度（调Y适应矮箱子）
    public float followOffsetX = 0.8f; // 箱子在玩家右侧的距离（负值=左侧）

    private Transform followedBox = null;

    void Update()
    {
        Vector2 detectionCenter = (Vector2)transform.position + boxCheckOffset;

        // 如果还没跟随箱子，尝试找一个
        if (followedBox == null)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(detectionCenter, detectionRadius, boxLayer);
            foreach (Collider2D col in hits)
            {
                if (col != null && col.attachedRigidbody != null)
                {
                    followedBox = col.transform;

                    // 关键：立即移到玩家旁边，避免重叠
                    SetBoxToFollowPosition();

                    // 关键：关闭物理，防止碰撞弹飞
                    Rigidbody2D rb = followedBox.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        rb.isKinematic = true;
                        rb.velocity = Vector2.zero;
                    }
                    break;
                }
            }
        }

        // 如果已锁定箱子，每帧同步位置
        if (followedBox != null)
        {
            SetBoxToFollowPosition();

            // 可选：如果箱子离太远（比如玩家跑太快），自动释放
            if (Vector2.Distance(followedBox.position, transform.position) > 3f)
            {
                ReleaseBox();
            }
        }

        // 调试可视化
        Debug.DrawRay(detectionCenter, Vector2.right * detectionRadius, Color.green);
        Debug.DrawRay(detectionCenter, Vector2.left * detectionRadius, Color.green);
        Debug.DrawLine(detectionCenter, detectionCenter + Vector2.up * 0.1f, Color.blue);
    }

    void SetBoxToFollowPosition()
    {
        Vector2 targetPos = transform.position;
        targetPos.x += followOffsetX; // 右侧为正，左侧为负
        followedBox.position = targetPos;
    }

    void ReleaseBox()
    {
        if (followedBox != null)
        {
            Rigidbody2D rb = followedBox.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
            followedBox = null;
        }
    }
}