using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SaveSlotManager : MonoBehaviour
{
    [Header("UI 引用")]
    public TextMeshProUGUI titleText;     // 顶部文字
    public GameObject[] saveSlots;        // 存档槽按钮
    public TextMeshProUGUI[] levelTexts;  // 每个存档的关卡文字
    public TextMeshProUGUI[] timeTexts;   // 每个存档的时间文字
    [SerializeField] private Button backButton;

    private SaveSlotMode currentMode;
    private GameObject fromPanel;
    private InGameUIManager uiManager;

    private void Awake()
    {
        uiManager = FindObjectOfType<InGameUIManager>();
    }

    public void Init(SaveSlotMode mode, GameObject fromPanel = null)
    {
        currentMode = mode;
        this.fromPanel = fromPanel;

        // 设置标题
        if (titleText != null)
            titleText.text = (mode == SaveSlotMode.Save) ? "保存存档" : "读取存档";

        // 设置返回按钮
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(OnBackClicked);
        }

        RefreshSlotsUI();

        // 绑定按钮事件
        for (int i = 0; i < saveSlots.Length; i++)
        {
            int index = i;
            Button btn = saveSlots[i].GetComponentInChildren<Button>();
            btn.onClick.RemoveAllListeners();

            if (mode == SaveSlotMode.Save)
                btn.onClick.AddListener(() => SaveSlot(index));
            else
                btn.onClick.AddListener(() => LoadSlot(index));
        }
    }

    private void RefreshSlotsUI()
    {
        for (int i = 0; i < saveSlots.Length; i++)
        {
            SaveData data = SaveSystem.Load(i);
            if (data != null)
            {
                if (levelTexts[i] != null)
                    levelTexts[i].text = $"关卡 {data.level}";
                if (timeTexts[i] != null)
                    timeTexts[i].text = data.saveTime;
            }
            else
            {
                if (levelTexts[i] != null)
                    levelTexts[i].text = "空存档";
                if (timeTexts[i] != null)
                    timeTexts[i].text = "-";
            }
        }
    }

    private void SaveSlot(int index)
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        SaveData data = new SaveData(currentLevel, 0);
        SaveSystem.Save(index, data);
        RefreshSlotsUI();

        Debug.Log($"✅ 存档 {index + 1}：保存关卡 {currentLevel}");
        OnBackClicked();
    }

    private void LoadSlot(int index)
    {
        SaveData data = SaveSystem.Load(index);
        if (data == null)
        {
            Debug.LogWarning($"⚠️ 存档槽 {index + 1} 为空！");
            return;
        }

        int totalScenes = SceneManager.sceneCountInBuildSettings;
        int nextLevel = data.level + 1;

        // 安全判断
        if (nextLevel < totalScenes)
        {
            Debug.Log($"🔄 读取存档 {index + 1}：从关卡 {data.level} 跳转到下一关（Index={nextLevel}）");
            Time.timeScale = 1f;
            SceneManager.LoadScene(nextLevel);
        }
        else
        {
            Debug.LogWarning($"⚠️ 已经是最后一个关卡（Index={data.level}），返回主菜单");
            Time.timeScale = 1f;
            SceneManager.LoadScene("Menu"); // 或可改成重载当前关卡
        }
    }

    private void OnBackClicked()
    {
        gameObject.SetActive(false);

        if (fromPanel != null)
            fromPanel.SetActive(true);
        else if (uiManager != null && uiManager.pauseMenuPanel != null)
            uiManager.pauseMenuPanel.SetActive(true);
    }
}
