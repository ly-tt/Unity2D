// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class PlayerSpawnManager : MonoBehaviour
// {
//     [Header("玩家出生点")]
//     [SerializeField] private Transform respawnPoint;

//     // Start is called before the first frame update
//     void Start()
//     {
//         if (respawnPoint != null)
//         {
//             transform.position = respawnPoint.position;
//             Debug.Log($"玩家已出生在复活点：{respawnPoint.position}");
//         }
//         else
//         {
//             Debug.LogWarning("未指定 RespawnPoint，玩家将在原始位置生成。");
//         }
//     }
// }

using System.Collections;
using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    [Header("玩家出生点")]
    [SerializeField] private Transform respawnPoint;
    [Header("闪光效果（可选）")]
    [SerializeField] private ParticleSystem spawnEffect;
    [Header("放大时间")]
    [SerializeField] private float spawnDuration = 0.8f;

    private void Start()
    {
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
            Debug.Log($"玩家已出生在复活点：{respawnPoint.position}");
            StartCoroutine(SpawnAnimation());
        }
        else
        {
            Debug.LogWarning("未指定 RespawnPoint，玩家将在原始位置生成。");
        }
    }

    private IEnumerator SpawnAnimation()
    {
        if (spawnEffect != null)
            spawnEffect.Play();

        // 记录原始缩放
        Vector3 originalScale = transform.localScale;

        // 初始为 0 倍大小
        transform.localScale = Vector3.zero;

        float elapsed = 0f;
        while (elapsed < spawnDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / spawnDuration;
            t = Mathf.Sin(t * Mathf.PI * 0.5f); // 平滑曲线
            transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);
            yield return null;
        }

        transform.localScale = originalScale;
    }

}

