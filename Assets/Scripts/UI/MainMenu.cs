using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [Header("菜单面板")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsMenuPanel;
    [SerializeField] private GameObject controlsPanel;

    [Header("主菜单按钮")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    [Header("按钮背景效果")]
    [SerializeField] private GameObject newGameButtonBg;
    [SerializeField] private GameObject continueButtonBg;
    [SerializeField] private GameObject settingsButtonBg;
    [SerializeField] private GameObject quitButtonBg;

    [Header("场景名称")]
    [SerializeField] private string firstLevelSceneName = "Level_0";

    private void Awake()
    {
        // 如果未在Inspector中赋值，尝试自动查找
        if (!mainMenuPanel) mainMenuPanel = GameObject.Find("MainMenuPanel");
        if (!settingsMenuPanel) settingsMenuPanel = GameObject.Find("SettingsMenuPanel");
        if (!controlsPanel) controlsPanel = GameObject.Find("ControlsPanel");

        // 查找按钮
        if (!newGameButton) newGameButton = GameObject.Find("NewGameButton")?.GetComponent<Button>();
        if (!continueButton) continueButton = GameObject.Find("ContinueButton")?.GetComponent<Button>();
        if (!settingsButton) settingsButton = GameObject.Find("SettingsButton")?.GetComponent<Button>();
        if (!quitButton) quitButton = GameObject.Find("QuitButton")?.GetComponent<Button>();

        // 查找按钮背景
        if (!newGameButtonBg) newGameButtonBg = GameObject.Find("NewGameButtonBg");
        if (!continueButtonBg) continueButtonBg = GameObject.Find("ContinueButtonBg");
        if (!settingsButtonBg) settingsButtonBg = GameObject.Find("SettingsButtonBg");
        if (!quitButtonBg) quitButtonBg = GameObject.Find("QuitButtonBg");
    }

    private void Start()
    {
        // 初始化界面状态
        ShowMainMenu();
        
        // 绑定主菜单按钮事件
        newGameButton.onClick.AddListener(OnNewGameClicked);
        continueButton.onClick.AddListener(OnContinueClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        quitButton.onClick.AddListener(OnQuitClicked);

        // 设置按钮悬停事件
        SetupButtonHoverEffects();

        // 检查存档
        CheckSaveExists();
        
        // 初始隐藏所有按钮背景
        HideAllButtonBackgrounds();
    }

    private void SetupButtonHoverEffects()
    {
        // 为主菜单按钮添加悬停效果
        AddHoverEffect(newGameButton, newGameButtonBg);
        AddHoverEffect(continueButton, continueButtonBg);
        AddHoverEffect(settingsButton, settingsButtonBg);
        AddHoverEffect(quitButton, quitButtonBg);
    }

    private void AddHoverEffect(Button button, GameObject background)
    {
        if (button == null || background == null) return;

        // 添加事件触发器
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        // 鼠标进入事件
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => { OnButtonHover(background, true); });
        trigger.triggers.Add(entryEnter);

        // 鼠标离开事件
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => { OnButtonHover(background, false); });
        trigger.triggers.Add(entryExit);
    }

    private void OnButtonHover(GameObject background, bool isHovering)
    {
        if (background != null)
            background.SetActive(isHovering);
    }

    private void HideAllButtonBackgrounds()
    {
        GameObject[] allBackgrounds = { newGameButtonBg, continueButtonBg, settingsButtonBg, quitButtonBg };
        
        foreach (var bg in allBackgrounds)
        {
            if (bg != null) bg.SetActive(false);
        }
    }

    #region 界面切换方法
    public void ShowMainMenu()
    {
        mainMenuPanel?.SetActive(true);
        settingsMenuPanel?.SetActive(false);
        controlsPanel?.SetActive(false);
    }

    public void ShowSettingsMenu()
    {
        mainMenuPanel?.SetActive(false);
        settingsMenuPanel?.SetActive(true);
        controlsPanel?.SetActive(false);
    }

    public void ShowControlsMenu()
    {
        mainMenuPanel?.SetActive(false);
        settingsMenuPanel?.SetActive(false);
        controlsPanel?.SetActive(true);
    }
    #endregion

    #region 按钮点击事件
    private void OnNewGameClicked()
    {
        Debug.Log("新游戏开始");
        SceneManager.LoadScene(firstLevelSceneName);
    }

    private void OnContinueClicked()
    {
        Debug.Log("继续游戏被点击");

        SaveData data = SaveSystem.Load(0);

        if (data != null)
        {
            int levelToLoad = data.level;
            int totalScenes = SceneManager.sceneCountInBuildSettings;

            if (levelToLoad < totalScenes)
            {
                Debug.Log($"加载自动存档：关卡 {levelToLoad}");
                SceneManager.LoadScene(levelToLoad);
            }
            else
            {
                Debug.LogWarning("存档关卡超出范围，加载第一个关卡。");
                SceneManager.LoadScene(firstLevelSceneName);
            }
        }
        else
        {
            Debug.Log("未找到自动存档，开始新游戏。");
            SceneManager.LoadScene(firstLevelSceneName);
        }
    }

    private void OnSettingsClicked()
    {
        Debug.Log("打开设置菜单");
        ShowSettingsMenu();
    }

    private void OnQuitClicked()
    {
        Debug.Log("退出游戏");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    #endregion

    private void CheckSaveExists()
    {
        bool hasSave = SaveSystem.Load(0) != null;
        if (continueButton != null)
        {
            continueButton.interactable = hasSave;

            // 灰化显示
            ColorBlock colors = continueButton.colors;
            colors.normalColor = hasSave ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.5f);
            continueButton.colors = colors;
        }
    }
}