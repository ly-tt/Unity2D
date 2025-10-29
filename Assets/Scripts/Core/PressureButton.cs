using UnityEngine;

public enum ButtonType
{
    Hold,
    Toggle
}

[RequireComponent(typeof(Collider2D))]
public class PressureButton : MonoBehaviour
{
    [Header("按钮类型")]
    public ButtonType buttonType = ButtonType.Hold;

    [Header("受控的激光")]
    public LaserController[] controlledLasers;

    [Header("按钮动画（可选）")]
    public Animator buttonAnimator;

    [Header("音效")]
    public AudioClip pressSound;      // 按下音效
    public AudioClip releaseSound;    // 松开音效
    public AudioSource audioSource;   // 拖入子物体上的 AudioSource

    [Header("检测设置")]
    public LayerMask playerAndBoxLayer;
    public float checkHeightOffset = 0.1f;
    public float checkDistance = 0.6f;

    private bool isPressed = false;
    private bool isPermanentlyOff = false;

    void Start()
    {
        // 安全检查：如果没有手动指定 audioSource，尝试自动查找子物体
        if (audioSource == null)
        {
            audioSource = GetComponentInChildren<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogWarning("PressureButton: No AudioSource assigned or found. Sound will be disabled.", this);
            }
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null && col.isTrigger)
        {
            // 可选：强制设为非 Trigger（射线检测不需要 Trigger）
            // col.isTrigger = false;
        }
    }

    void Update()
    {
        CheckForObjectsOnTop();
    }

    void CheckForObjectsOnTop()
    {
        Vector2 rayOrigin = (Vector2)transform.position + Vector2.up * checkHeightOffset;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, checkDistance, playerAndBoxLayer);

        bool somethingOnTop = hit.collider != null;

        if (somethingOnTop && !isPressed)
        {
            isPressed = true;
            OnPressed();
        }
        else if (!somethingOnTop && isPressed)
        {
            isPressed = false;
            OnReleased();
        }
    }

    bool IsInteractable(Collider2D collider)
    {
        return collider.CompareTag("Player") || collider.CompareTag("Box");
    }

    void OnPressed()
    {
        if (buttonType == ButtonType.Toggle)
        {
            if (!isPermanentlyOff)
            {
                isPermanentlyOff = true;
                SetLasersActive(false);
                PlayButtonPress();
            }
        }
        else // Hold
        {
            SetLasersActive(false);
            PlayButtonPress();
        }
    }

    void OnReleased()
    {
        if (buttonType == ButtonType.Hold)
        {
            if (!isPermanentlyOff)
            {
                SetLasersActive(true);
                PlayButtonRelease();
            }
        }
    }

    void SetLasersActive(bool active)
    {
        if (controlledLasers == null) return;

        foreach (var laser in controlledLasers)
        {
            if (laser != null)
            {
                if (active)
                    laser.TurnOn();
                else
                    laser.TurnOff();
            }
        }
    }

    void PlayButtonPress()
    {
        // 触发动画
        buttonAnimator?.SetTrigger("Press");
        // 播放音效
        if (audioSource != null && pressSound != null)
        {
            audioSource.PlayOneShot(pressSound);
        }
    }

    void PlayButtonRelease()
    {
        // 触发动画
        buttonAnimator?.SetTrigger("Release");
        // 播放音效
        if (audioSource != null && releaseSound != null)
        {
            audioSource.PlayOneShot(releaseSound);
        }
    }

    void OnDrawGizmos()
    {
        Vector2 start = (Vector2)transform.position + Vector2.up * checkHeightOffset;
        Gizmos.color = isPressed ? Color.green : Color.red;
        Gizmos.DrawLine(start, start + Vector2.up * checkDistance);

        if (controlledLasers != null)
        {
            Gizmos.color = Color.yellow;
            foreach (var laser in controlledLasers)
            {
                if (laser != null)
                {
                    Gizmos.DrawLine(transform.position, laser.transform.position);
                }
            }
        }
    }
}