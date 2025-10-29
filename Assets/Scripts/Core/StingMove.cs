using UnityEngine;

public class RectangularMover : MonoBehaviour
{
    [Header("路径点（按顺时针顺序：左上 → 右上 → 右下 → 左下）")]
    public Vector2[] waypoints = new Vector2[4]
    {
        new Vector2(-2, 2),  // 左上
        new Vector2(2, 2),   // 右上
        new Vector2(2, -2),  // 右下
        new Vector2(-2, -2)  // 左下
    };

    [Header("移动设置")]
    public float speed = 2f; // 单位/秒

    private int currentTargetIndex = 0;
    private Vector2 startPosition;

    void Start()
    {
        // 初始化起点为第一个路径点（左上）
        startPosition = waypoints[0];
        transform.position = startPosition;
    }

    void Update()
    {
        if (waypoints.Length < 4)
        {
            Debug.LogWarning("RectangularMover 需要至少4个路径点！", this);
            return;
        }

        Vector2 target = waypoints[currentTargetIndex];
        Vector2 currentPosition = transform.position;

        // 计算到目标点的方向和距离
        Vector2 direction = target - currentPosition;
        float distanceThisFrame = speed * Time.deltaTime;

        // 如果即将到达目标点
        if (direction.magnitude <= distanceThisFrame)
        {
            // 精确到达目标
            transform.position = target;
            // 切换到下一个点（循环）
            currentTargetIndex = (currentTargetIndex + 1) % waypoints.Length;
        }
        else
        {
            // 向目标点匀速移动
            transform.position = currentPosition + direction.normalized * distanceThisFrame;
        }

        // 关键：保持原始朝向（不旋转）
        // 如果你的物体初始有旋转，这里保留它；否则保持为 Quaternion.identity
        // 由于我们只改 position，rotation 不变，所以无需额外操作
    }

    // 可视化路径（仅在 Scene 视图中）
    void OnDrawGizmos()
    {
        if (waypoints.Length < 4) return;

        Gizmos.color = Color.cyan;
        Vector3 prev = GetWorldPoint(waypoints[waypoints.Length - 1]);
        for (int i = 0; i < waypoints.Length; i++)
        {
            Vector3 current = GetWorldPoint(waypoints[i]);
            Gizmos.DrawLine(prev, current);
            prev = current;
        }

        // 绘制点
        for (int i = 0; i < waypoints.Length; i++)
        {
            Gizmos.color = i == 0 ? Color.green : Color.yellow;
            Gizmos.DrawSphere(GetWorldPoint(waypoints[i]), 0.1f);
        }
    }

    Vector3 GetWorldPoint(Vector2 localPoint)
    {
        // 如果该脚本挂在根对象上，且路径点是世界坐标，则直接用
        // 如果你想支持局部坐标（比如路径相对于父物体），可改为 transform.TransformPoint(localPoint)
        return localPoint;
    }
}