using UnityEngine;

public class BoxController : MonoBehaviour
{
    [Header("Box Settings")]
    public float mass = 2f; // 箱子质量
    public float friction = 0.8f; // 摩擦力

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        // 设置箱子物理属性
        rb.mass = mass;
        rb.drag = friction;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // 防止箱子旋转
    }

    void Update()
    {
        // 可以在这里添加箱子的其他逻辑
    }
}