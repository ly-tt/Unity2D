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

    [Header("检测设置")]
    public LayerMask playerAndBoxLayer; // 在 Inspector 中指定包含 Player 和 Box 的 Layer
    public float checkHeightOffset = 0.1f; // 从按钮表面往上偏移
    public float checkDistance = 0.6f;     // 射线长度（应略大于角色脚底到重心距离）

    private bool isPressed = false;
    private bool isPermanentlyOff = false;

    void Start()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && col.isTrigger)
        {
            // 可以保留为 Trigger（用于可视化或其它用途），但不用于逻辑判断
            // 或者直接设为非 Trigger（推荐）
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
        buttonAnimator?.SetTrigger("Press");
    }

    void PlayButtonRelease()
    {
        buttonAnimator?.SetTrigger("Release");
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