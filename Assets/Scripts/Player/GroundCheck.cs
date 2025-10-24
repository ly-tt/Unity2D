using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [Header("检测参数")]
    [SerializeField] private Transform playerTransform;   // Player Transform
    [SerializeField] private Vector2 offset = new Vector2(0, -0.5f); // 相对于 Player 的偏移
    [SerializeField] private float radius = 0.2f;         // 检测半径
    [SerializeField] private LayerMask groundLayer;       // 地面 Layer

    [Header("调试")]
    [SerializeField] private bool showGizmos = true;

    // 当前是否接触地面
    public bool IsGrounded { get; private set; }

    void Update()
    {
        // 检测地面
        Vector2 checkPos = (Vector2)playerTransform.position + offset;
        IsGrounded = Physics2D.OverlapCircle(checkPos, radius, groundLayer);
    }

    // Scene 视图绘制检测范围
    void OnDrawGizmosSelected()
    {
        if (!showGizmos || playerTransform == null) return;

        Gizmos.color = Color.yellow;
        Vector2 checkPos = (Vector2)playerTransform.position + offset;
        Gizmos.DrawWireSphere(checkPos, radius);
    }
}