using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // 玩家角色
    public float smoothSpeed = 0.125f; // 平滑移动
    public Vector2 mapMin; // 地图左下角坐标
    public Vector2 mapMax; // 地图右上角坐标

    private float camHalfHeight;
    private float camHalfWidth;

    void Start()
    {
        // 计算摄像机视野半径
        Camera cam = GetComponent<Camera>();
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = cam.aspect * camHalfHeight;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // 目标位置：跟随 player 的 x
        Vector3 targetPos = new Vector3(player.position.x, player.position.y, transform.position.z);

        // 限制摄像头不超出地图边界
        float clampedX = Mathf.Clamp(targetPos.x, mapMin.x + camHalfWidth, mapMax.x - camHalfWidth);
        float clampedY = Mathf.Clamp(targetPos.y, mapMin.y + camHalfHeight, mapMax.y - camHalfHeight);

        // 只跟随 X 轴
        Vector3 desiredPos = new Vector3(clampedX, transform.position.y, transform.position.z);

        // 平滑移动
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
    }
}
