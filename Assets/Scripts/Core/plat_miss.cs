using System.Collections;
using UnityEngine;

public class DisappearingPlatform : MonoBehaviour
{
    [Header("平台设置")]
    public float disappearTime = 3f;
    public float respawnTime = 5f;

    private bool isPlayerOnPlatform = false;
    private bool isDisappeared = false;
    private Coroutine disappearCoroutine;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDisappeared || !collision.gameObject.CompareTag("Player"))
            return;

        isPlayerOnPlatform = true;

        if (disappearCoroutine != null)
            StopCoroutine(disappearCoroutine);

        disappearCoroutine = StartCoroutine(DisappearAfterDelay());
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnPlatform = false;
            if (disappearCoroutine != null)
            {
                StopCoroutine(disappearCoroutine);
                disappearCoroutine = null;
            }
        }
    }

    private IEnumerator DisappearAfterDelay()
    {
        yield return new WaitForSeconds(disappearTime);

        if (isPlayerOnPlatform && !isDisappeared)
        {
            Disappear();
        }
        else
        {
            disappearCoroutine = null;
        }
    }

    private void Disappear()
    {
        isDisappeared = true;
        isPlayerOnPlatform = false;
        disappearCoroutine = null;

        gameObject.SetActive(false);

        // ✅ 委托给全局管理器处理重生（它永远不会被禁用）
        PlatformManager.Instance.ScheduleRespawn(gameObject, respawnTime);
    }

    // 提供一个安全的重置方法
    public void ResetState()
    {
        isDisappeared = false;
        // isPlayerOnPlatform 会在下次 OnCollisionEnter 时自动设为 true
    }
}