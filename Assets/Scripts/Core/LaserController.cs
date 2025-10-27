using UnityEngine;

[RequireComponent(typeof(Animator))]
public class LaserController : MonoBehaviour
{
    [Tooltip("游戏开始时激光是否开启")]
    public bool startsOn = true;

    private Animator animator;
    private static readonly int IsOnHash = Animator.StringToHash("IsOn");

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("LaserController 需要 Animator 组件！", this);
            enabled = false;
            return;
        }
    }

    void Start()
    {
        // 初始化状态
        SetLaserActive(startsOn);
    }

    public void TurnOn()
    {
        SetLaserActive(true);
    }

    public void TurnOff()
    {
        SetLaserActive(false);
    }

    private void SetLaserActive(bool active)
    {
        animator.SetBool(IsOnHash, active);
    }
}