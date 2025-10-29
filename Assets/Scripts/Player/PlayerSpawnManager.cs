using System.Collections;
using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    [Header("玩家出生点")]
    [SerializeField] private Transform respawnPoint;

    [Header("出生动画帧")]
    [SerializeField] private SpriteRenderer effectRenderer;
    [SerializeField] private Sprite[] lightningFrames;

    [Header("放大时间")]
    [SerializeField] private float spawnDuration = 0.8f;

    [Header("闪电帧间隔")]
    [SerializeField] private float frameTime = 0.08f;

    private void Start()
    {
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
            Debug.Log($"玩家已出生在复活点：{respawnPoint.position}");
            StartCoroutine(SpawnSequence());
        }
        else
        {
            Debug.LogWarning("未指定 RespawnPoint，玩家将在原始位置生成。");
        }
    }

    private IEnumerator SpawnSequence()
    {
        // 初始化缩放
        Vector3 originalScale = transform.localScale;
        transform.localScale = Vector3.zero;

        // 并行执行：闪电特效 + 放大动画
        IEnumerator lightning = PlayLightningEffect();
        IEnumerator grow = PlayerGrowAnimation(originalScale);

        // 同时启动两个协程
        StartCoroutine(lightning);
        yield return StartCoroutine(grow);

        // 确保最后特效消失
        if (effectRenderer != null)
            effectRenderer.sprite = null;
    }

    private IEnumerator PlayLightningEffect()
    {
        if (effectRenderer == null || lightningFrames.Length == 0)
            yield break;

        foreach (var frame in lightningFrames)
        {
            effectRenderer.sprite = frame;
            yield return new WaitForSeconds(frameTime);
        }
    }

    private IEnumerator PlayerGrowAnimation(Vector3 targetScale)
    {
        float elapsed = 0f;
        while (elapsed < spawnDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Sin((elapsed / spawnDuration) * Mathf.PI * 0.5f); // 平滑放大
            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
            yield return null;
        }
        transform.localScale = targetScale;
    }
}
