using UnityEngine;

public class OscillatingPlatform : MonoBehaviour
{
    [Header("移动设置")]
    public Vector2 leftPoint;   // 左侧终点（世界坐标）
    public Vector2 rightPoint;  // 右侧终点（世界坐标）
    public float speed = 2f;    // 移动速度（单位/秒）

    private Vector2 currentTarget;
    private bool movingRight = true;

    void OnEnable()
    {
        // 每次启用时（包括初始启动和重生），重置到左侧起点
        transform.position = leftPoint;
        currentTarget = rightPoint;
        movingRight = true;
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector2 currentPosition = transform.position;
        Vector2 direction = (currentTarget - currentPosition).normalized;
        float distanceThisFrame = speed * Time.deltaTime;

        // 检查是否到达目标点
        if (Vector2.Distance(currentPosition, currentTarget) <= distanceThisFrame)
        {
            // 精确到达
            transform.position = currentTarget;

            // 切换方向
            if (movingRight)
            {
                currentTarget = leftPoint;
                movingRight = false;
            }
            else
            {
                currentTarget = rightPoint;
                movingRight = true;
            }
        }
        else
        {
            // 继续移动
            transform.position = currentPosition + direction * distanceThisFrame;
        }
    }

    // Scene 视图中可视化路径
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(leftPoint, rightPoint);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(leftPoint, 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(rightPoint, 0.1f);
    }
}