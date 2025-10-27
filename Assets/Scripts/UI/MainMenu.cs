using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// 主菜单管理器 - 负责主菜单界面的显示和交互

public class MainMenu : MonoBehaviour
{
    [Header("菜单面板名称")]
    [SerializeField] private string mainMenuPanelName = "MainMenuPanel";
    [SerializeField] private string saveSlotMenuPanelName = "SaveSlotMenuPanel";
    [SerializeField] private string settingsMenuPanelName = "SettingsMenuPanel";

    [Header("按钮名称")]
    [SerializeField] private string newGameButtonName = "NewGameButton";
    [SerializeField] private string continueButtonName = "ContinueButton";
    [SerializeField] private string settingsButtonName = "SettingsButton";
    [SerializeField] private string quitButtonName = "QuitButton";

    [Header("场景名称")]
    [SerializeField] private string gameSceneName = "testLevel02";

    // 运行时引用
    private GameObject mainMenuPanel;
    private GameObject saveSlotMenuPanel;
    private GameObject settingsMenuPanel;
    private Button newGameButton;
    private Button continueButton;
    private Button settingsButton;
    private Button quitButton;

    private void Awake()
    {
        // 自动查找 GameObject
        mainMenuPanel = GameObject.Find(mainMenuPanelName);
        saveSlotMenuPanel = GameObject.Find(saveSlotMenuPanelName);
        settingsMenuPanel = GameObject.Find(settingsMenuPanelName);

        // 自动查找 Button
        newGameButton = GameObject.Find(newGameButtonName)?.GetComponent<Button>();
        continueButton = GameObject.Find(continueButtonName)?.GetComponent<Button>();
        settingsButton = GameObject.Find(settingsButtonName)?.GetComponent<Button>();
        quitButton = GameObject.Find(quitButtonName)?.GetComponent<Button>();

        // 引用检查
        if (!mainMenuPanel || !saveSlotMenuPanel || !settingsMenuPanel)
            Debug.LogError("有面板对象未找到，请检查名称是否匹配。");
        if (!newGameButton || !continueButton || !settingsButton || !quitButton)
            Debug.LogError("有按钮对象未找到，请检查名称是否匹配。");
    }

    private void Start()
    {
        ShowMainMenu();

        // 绑定按钮事件
        newGameButton.onClick.AddListener(OnNewGameClicked);
        continueButton.onClick.AddListener(OnContinueClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        quitButton.onClick.AddListener(OnQuitClicked);

        CheckSaveExists();
    }

    public void ShowMainMenu()
    {
        mainMenuPanel?.SetActive(true);
        saveSlotMenuPanel?.SetActive(false);
        settingsMenuPanel?.SetActive(false);
    }

    private void OnNewGameClicked()
    {
        Debug.Log("新游戏按钮被点击");
        SceneManager.LoadScene(gameSceneName);
    }

    private void OnContinueClicked()
    {
        Debug.Log("继续游戏按钮被点击");
        ShowSaveSlotMenu();
    }

    private void OnSettingsClicked()
    {
        Debug.Log("设置按钮被点击");
        ShowSettingsMenu();
    }

    private void OnQuitClicked()
    {
        Debug.Log("退出游戏按钮被点击");

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    private void ShowSaveSlotMenu()
    {
        if (saveSlotMenuPanel == null)
        {
            Debug.LogError("⚠️ saveSlotMenuPanel 未找到，请检查名称！");
            return;
        }

        mainMenuPanel?.SetActive(false);
        saveSlotMenuPanel.SetActive(true);

        var menu = saveSlotMenuPanel.GetComponent<SaveSlotMenu>();
        if (menu != null)
            menu.mainMenuPanel = mainMenuPanel;
        else
            Debug.LogError("⚠️ saveSlotMenuPanel 上未找到 SaveSlotMenu 脚本！");
    }

    private void ShowSettingsMenu()
    {
        mainMenuPanel?.SetActive(false);
        settingsMenuPanel?.SetActive(true);
    }

    private void CheckSaveExists()
    {
        bool hasSave = false;

        for (int i = 0; i < 3; i++) // 三个存档槽
        {
            if (SaveSystem.Load(i) != null)
            {
                hasSave = true;
                break;
            }
        }

        continueButton.interactable = hasSave;

        ColorBlock colors = continueButton.colors;
        colors.normalColor = hasSave
            ? Color.white
            : new Color(0.5f, 0.5f, 0.5f, 0.5f);
        continueButton.colors = colors;
    }

}