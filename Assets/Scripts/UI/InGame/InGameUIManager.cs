using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public enum SaveSlotMode { Save, Load }

public class InGameUIManager : MonoBehaviour
{
    [Header("核心面板")]
    public GameObject hudPanel;
    public GameObject levelCompletePanel;
    public GameObject deathPanel;
    public GameObject pauseMenuPanel;
    public GameObject saveSlotMenuPanel;
    public GameObject settingsMenuPanel;
    public GameObject controlsMenuPanel;
    public GameObject confirmationPanel;

    [Header("HUD 元素")]
    public TextMeshProUGUI levelNameText;


    [Header("通用UI元素")]
    public TextMeshProUGUI messageText;
    public Button confirmButton;
    public Button cancelButton;

    [Header("LevelComplete按钮")]
    public Button nextLevelButton;
    public Button restartLevelButton;

    private void Start()
    {
        HideAllPanels();
        hudPanel.SetActive(true);
        UpdateLevelName();

        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(() => {
                Time.timeScale = 1f;
                LoadNextLevel();
            });

        if (restartLevelButton != null)
            restartLevelButton.onClick.AddListener(() => {
                Time.timeScale = 1f;
                RestartLevel();
            });
    }

    public void HideAllPanels()
    {
        if (hudPanel != null) hudPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        if (deathPanel != null) deathPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (saveSlotMenuPanel != null) saveSlotMenuPanel.SetActive(false);
        if (settingsMenuPanel != null) settingsMenuPanel.SetActive(false);
        if (controlsMenuPanel != null) controlsMenuPanel.SetActive(false);
        if (confirmationPanel != null) confirmationPanel.SetActive(false);
    }

    public void UpdateLevelName()
    {
        if (levelNameText == null) return;

        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        levelNameText.text = $"第 {currentLevel} 关";
    }

    // ===== 通关 / 死亡 =====
    public void ShowLevelComplete()
    {
        HideAllPanels();
        if (levelCompletePanel != null) levelCompletePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ShowDeathPanel()
    {
        HideAllPanels();
        if (deathPanel != null) deathPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // ===== 暂停 =====
    public void PauseGame()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // ===== 面板跳转 =====
    public void OpenSettings()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsMenuPanel != null) settingsMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OpenControls()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (controlsMenuPanel != null) controlsMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // ✅ ===== 打开存档菜单（改进版） =====
    public void OpenSaveMenu(SaveSlotMode mode, GameObject fromPanel = null)
    {
        // ① 隐藏来源面板
        if (fromPanel != null)
        {
            fromPanel.SetActive(false);
            Debug.Log($"隐藏来源面板：{fromPanel.name}");
        }
        else if (pauseMenuPanel != null && pauseMenuPanel.activeSelf)
        {
            pauseMenuPanel.SetActive(false);
            Debug.Log("默认隐藏暂停菜单");
        }

        // ② 打开存档面板
        if (saveSlotMenuPanel != null)
        {
            saveSlotMenuPanel.SetActive(true);
            var slotManager = saveSlotMenuPanel.GetComponent<SaveSlotManager>();
            if (slotManager != null)
            {
                slotManager.Init(mode, fromPanel); // ✅ 保存来源面板引用
                Debug.Log($"初始化存档菜单，来源面板：{fromPanel?.name ?? "无"}");
            }
            else
            {
                Debug.LogError("⚠️ saveSlotMenuPanel 上未找到 SaveSlotManager 组件！");
            }
        }
        else
        {
            Debug.LogError("⚠️ 未设置 saveSlotMenuPanel！");
        }

        // ③ 暂停游戏
        Time.timeScale = 0f;
    }

    // ===== 确认框 =====
    public void ShowConfirmation(string message, Action onConfirm, GameObject fromPanel = null)
{
    // ① 隐藏来源面板
    GameObject panelToHide = fromPanel ?? pauseMenuPanel; // 默认隐藏暂停菜单
    if (panelToHide != null && panelToHide.activeSelf)
    {
        panelToHide.SetActive(false);
        Debug.Log($"隐藏来源面板：{panelToHide.name}");
    }

    // ② 显示确认框
    if (confirmationPanel != null)
        confirmationPanel.SetActive(true);

    if (messageText != null)
        messageText.text = message;

    // ③ 设置按钮事件
    if (confirmButton != null)
    {
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            if (confirmationPanel != null) confirmationPanel.SetActive(false);
            onConfirm?.Invoke();
            // 恢复之前的来源面板（可选）
            if (panelToHide != null) panelToHide.SetActive(true);
        });
    }

    if (cancelButton != null)
    {
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(() =>
        {
            if (confirmationPanel != null) confirmationPanel.SetActive(false);
            // 恢复之前的来源面板
            if (panelToHide != null) panelToHide.SetActive(true);
        });
    }

    // ④ 暂停游戏
    Time.timeScale = 0f;
}


    // ===== 场景操作 =====
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        if (next < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(next);
        else
            ReturnToMenu();
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    private void Update()
    {
        // ESC 键暂停/恢复游戏
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenuPanel != null && pauseMenuPanel.activeInHierarchy)
            {
                ResumeGame();
            }
            else if (!IsAnyUIPanelActive()) // 确保没有其他UI打开时才能暂停
            {
                PauseGame();
            }
        }
    }

    private bool IsAnyUIPanelActive()
    {
        return (levelCompletePanel != null && levelCompletePanel.activeInHierarchy) ||
               (deathPanel != null && deathPanel.activeInHierarchy) ||
               (saveSlotMenuPanel != null && saveSlotMenuPanel.activeInHierarchy) ||
               (settingsMenuPanel != null && settingsMenuPanel.activeInHierarchy) ||
               (controlsMenuPanel != null && controlsMenuPanel.activeInHierarchy) ||
               (confirmationPanel != null && confirmationPanel.activeInHierarchy);
    }
}