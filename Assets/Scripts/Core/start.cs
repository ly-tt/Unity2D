using UnityEngine;

public class StarFragment : MonoBehaviour
{
    public enum FragmentType
    {
        PlayerGravity,  // 碎片1：玩家重力反转
        BoxGravity      // 碎片2：箱子重力反转
    }

    public FragmentType type;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检测是否是玩家（你可以用 Tag 或组件判断）
        if (other.CompareTag("Player")) // ← 确保你的玩家 GameObject 的 Tag 是 "Player"
        {
            PlayerScript player = other.GetComponent<PlayerScript>();
            if (player != null)
            {
                // 根据类型解锁对应能力
                switch (type)
                {
                    case FragmentType.PlayerGravity:
                        player.hasGravityInvertAbility = true;
                        Debug.Log("获得玩家重力反转能力！");
                        break;
                    case FragmentType.BoxGravity:
                        player.hasBoxGravityInvertAbility = true;
                        Debug.Log("获得箱子重力反转能力！");
                        break;
                }

                // 碎片消失（不再重现）
                Destroy(gameObject);
            }
        }
    }
}