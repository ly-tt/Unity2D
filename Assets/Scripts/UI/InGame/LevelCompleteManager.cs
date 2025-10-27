using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelCompleteManager : MonoBehaviour
{
    [Header("UI 元素")]
    public GameObject panel;
    public TMP_Text titleText;
    public Button saveButton;
    public Button nextLevelButton;
    public Button renewButton;

    [Header("下一关场景设置")]
    public string nextLevelSceneName;

    private InGameUIManager uiManager;

    private void Awake()
    {
        if (panel != null)
            panel.SetActive(false);

        if (saveButton != null)
            saveButton.onClick.AddListener(OnSaveGame);
        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(OnNextLevel);
        if (renewButton != null)
            renewButton.onClick.AddListener(OnRenewLevel);
    }

    private void Start()
    {
        uiManager = FindObjectOfType<InGameUIManager>();
        if (uiManager == null)
            Debug.LogWarning("⚠️ 未找到 InGameUIManager，保存菜单无法打开。");
    }

    public void ShowPanel()
    {
        if (panel == null)
        {
            Debug.LogError("未设置通关面板！");
            return;
        }

        panel.SetActive(true);
        Time.timeScale = 0f;

        if (titleText != null)
            titleText.text = "关卡完成！";
    }

    private void OnSaveGame()
    {
        if (uiManager != null)
        {
            uiManager.OpenSaveMenu(SaveSlotMode.Save, panel);
        }
        else
        {
            Debug.LogError("⚠️ 无法打开存档面板：未找到 InGameUIManager。");
        }
    }

    private void OnNextLevel()
    {
        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(nextLevelSceneName))
        {
            if (SceneExists(nextLevelSceneName))
            {
                SceneManager.LoadScene(nextLevelSceneName);
            }
            else
            {
                Debug.LogError($"⚠️ 场景 '{nextLevelSceneName}' 未添加到 Build Settings！");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ 未设置下一关场景名称！");
        }
    }

    private void OnRenewLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private bool SceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (name == sceneName)
                return true;
        }
        return false;
    }
}
