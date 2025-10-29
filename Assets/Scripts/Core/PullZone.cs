// PullZone.cs
using UnityEngine;

public class PullZone : MonoBehaviour
{
    public bool isLeftZone = true; // true 表示这是左侧交互区
    private PlayerScript player;

    void Start()
    {
        player = transform.root.GetComponent<PlayerScript>();
        if (player == null)
        {
            Debug.LogError("PullZone 必须作为 Player 的子物体使用！");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (player == null) return;
        // 检查是否是箱子层
        if ((player.boxLayer.value & (1 << other.gameObject.layer)) == 0) return;

        Rigidbody2D rb = other.attachedRigidbody;
        if (rb != null)
        {
            if (isLeftZone)
                player.SetBoxInLeftZone(rb);
            else
                player.SetBoxInRightZone(rb);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (player == null) return;
        if ((player.boxLayer.value & (1 << other.gameObject.layer)) == 0) return;

        Rigidbody2D rb = other.attachedRigidbody;
        if (rb != null)
        {
            if (isLeftZone)
                player.SetBoxInLeftZone(null);
            else
                player.SetBoxInRightZone(null);
        }
    }
}