using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    [Header("死亡UI")]
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private Button respawnButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private TextMeshProUGUI deathMessage;

    [Header("玩家引用")]
    public GameObject player;

    private PlayerDeath playerDeathHandler;

    private void Awake()
    {
        // 初始化只在Awake中执行一次
        if (deathPanel != null && deathPanel.activeSelf)
        {
            deathPanel.SetActive(false);
            Debug.Log("死亡面板初始化隐藏 (Awake)");
        }
    }

    void Start()
    {
        if (player != null)
        {
            playerDeathHandler = player.GetComponent<PlayerDeath>();
            if (playerDeathHandler == null)
                Debug.LogError("玩家对象上没有找到 PlayerDeath 组件！");
        }
        else
        {
            Debug.LogError("玩家引用未赋值！");
        }

        if (respawnButton != null)
            respawnButton.onClick.AddListener(OnRespawnClicked);
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
    }

    public void ShowDeathPanel(string message = "Game Over!")
    {
        Debug.Log($"开始显示死亡面板，deathPanel 是否为null: {deathPanel == null}");
        if (deathPanel == null)
        {
            Debug.LogError("死亡面板未赋值！");
            return;
        }

        Debug.Log($"显示前死亡面板状态: {deathPanel.activeSelf}");
        deathPanel.SetActive(true);
        Debug.Log($"显示后死亡面板状态: {deathPanel.activeSelf}");

        Time.timeScale = 0f;

        if (deathMessage != null)
            deathMessage.text = message;

        Canvas.ForceUpdateCanvases();
        Debug.Log("死亡面板显示完成");
    }

    /// <summary>
    /// 点击复活按钮：重新加载当前关卡，等价于挑战成功的“重新挑战”
    /// </summary>
    private void OnRespawnClicked()
    {
        Debug.Log("复活按钮被点击");
        
        // 恢复游戏时间
        Time.timeScale = 1f;

        // 可选：隐藏死亡面板，防止加载延迟造成视觉闪烁
        if (deathPanel != null)
            deathPanel.SetActive(false);

        // 重新加载当前场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnQuitClicked()
    {
        Debug.Log("退出按钮被点击");
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
