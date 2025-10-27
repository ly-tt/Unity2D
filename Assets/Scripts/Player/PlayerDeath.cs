using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [Header("死亡检测对象")]
    public Collider2D[] deathTriggers; // 可以设置多个死亡触发器
    
    [Header("死亡菜单引用")]
    public DeathMenu deathMenu; // 直接引用 DeathMenu 组件

    private bool isDead = false;
    private Rigidbody2D rb;
    private MoveJump moveJump;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveJump = GetComponent<MoveJump>();
        
        if (deathMenu == null)
        {
            Debug.LogError("DeathMenu 引用未赋值！请在 Inspector 中分配 DeathMenu 组件。");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        // 检测是否碰到了死亡触发器
        if (IsDeathTrigger(other))
        {
            Debug.Log($"碰到死亡触发器: {other.name}");
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        // 也可以通过碰撞触发死亡
        if (IsDeathTrigger(collision.collider))
        {
            Debug.Log($"碰撞到死亡物体: {collision.collider.name}");
            Die();
        }
    }

    /// <summary>
    /// 检查碰撞体是否为死亡触发器
    /// </summary>
    private bool IsDeathTrigger(Collider2D collider)
    {
        // 检查是否在指定的死亡触发器列表中
        if (deathTriggers != null && deathTriggers.Length > 0)
        {
            foreach (Collider2D deathTrigger in deathTriggers)
            {
                if (deathTrigger != null && collider == deathTrigger)
                {
                    return true;
                }
            }
        }
        
        return false;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("玩家死亡");

        // 禁止移动与物理
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }
        if (moveJump != null)
            moveJump.enabled = false;

        // 通知 DeathMenu 显示死亡面板
        if (deathMenu != null)
        {
            deathMenu.ShowDeathPanel("game over！");
        }
        else
        {
            Debug.LogError("未找到 DeathMenu！无法显示死亡面板");
        }
    }

    /// <summary>
    /// 复活玩家（由 DeathMenu 的复活按钮调用）
    /// </summary>
    public void Respawn()
    {
        isDead = false;

        // 恢复移动与物理
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = Vector2.zero;
        }
        if (moveJump != null)
            moveJump.enabled = true;

        Debug.Log("玩家复活");
    }
}