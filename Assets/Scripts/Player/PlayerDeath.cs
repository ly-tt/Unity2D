using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PlayerDeathHandler : MonoBehaviour
{
    [Header("死亡检测参数")]
    [SerializeField] private float deathYThreshold = -10f;
    [SerializeField] private Transform respawnPoint;

    [Header("死亡UI")]
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private Button respawnButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private TextMeshProUGUI deathMessage;

    private bool isDead = false;
    private Rigidbody2D rb;
    private MoveJump moveJump;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveJump = GetComponent<MoveJump>();

        if (respawnPoint == null)
            respawnPoint = transform;

        if (deathPanel != null)
            deathPanel.SetActive(false);

        // 确保有EventSystem
        EnsureEventSystem();

        if (respawnButton != null)
        {
            respawnButton.onClick.RemoveAllListeners();
            respawnButton.onClick.AddListener(Respawn);
            respawnButton.interactable = true;
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(ReturnToMenu);
            quitButton.interactable = true;
        }
    }

    void Update()
    {
        if (!isDead && transform.position.y < deathYThreshold)
        {
            Die();
        }
    }

    private void EnsureEventSystem()
    {
        // 确保场景中有EventSystem
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
            Debug.Log("创建了新的EventSystem");
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("开始执行 Die() 方法");

        // 确保EventSystem正常工作
        EnsureEventSystem();

        if (deathPanel != null)
        {
            // 先确保Canvas是最新的
            Canvas canvas = deathPanel.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                canvas.enabled = false;
                canvas.enabled = true;
            }

            deathPanel.SetActive(true);
            Debug.Log("死亡面板已激活");

            // 强制刷新UI
            StartCoroutine(RefreshUIAfterFrame());
            
            if (deathMessage != null)
            {
                deathMessage.text = "Game Over!";
            }
        }

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        if (moveJump != null)
        {
            moveJump.enabled = false;
        }
    }

    private IEnumerator RefreshUIAfterFrame()
    {
        // 等待一帧让UI完全初始化
        yield return null;
        
        // 强制重建布局
        if (deathPanel != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(deathPanel.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
        }

        // 确保按钮可交互并选中
        if (respawnButton != null)
        {
            respawnButton.interactable = true;
            respawnButton.Select();
            respawnButton.OnSelect(null);
            Debug.Log("复活按钮状态: " + respawnButton.interactable);
        }

        if (quitButton != null)
        {
            quitButton.interactable = true;
        }

        // 测试按钮点击
        TestButtonClick();
    }

    private void TestButtonClick()
    {
        // 添加测试方法到按钮
        if (respawnButton != null)
        {
            respawnButton.onClick.RemoveAllListeners();
            respawnButton.onClick.AddListener(() => {
                Debug.Log("测试: 复活按钮被点击!");
                Respawn();
            });
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(() => {
                Debug.Log("测试: 退出按钮被点击!");
                ReturnToMenu();
            });
        }
    }

    public void Respawn()
    {
        Debug.Log("Respawn() 方法被调用");
        isDead = false;

        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }

        transform.position = respawnPoint.position;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = Vector2.zero;
        }

        if (moveJump != null)
        {
            moveJump.enabled = true;
        }
    }

    public void ReturnToMenu()
    {
        Debug.Log("返回主菜单按钮被点击");
        
        // 恢复时间尺度（如果有暂停）
        Time.timeScale = 1f;
        
        string mainMenuSceneName = "Menu";
        
        // 检查场景是否存在
        if (Application.CanStreamedLevelBeLoaded(mainMenuSceneName))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogError($"场景 '{mainMenuSceneName}' 不存在或未添加到构建设置中！");
            
            // 提供备用方案：重新加载当前场景
            Debug.Log("将重新加载当前场景作为备用方案");
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}