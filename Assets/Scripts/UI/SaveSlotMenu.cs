using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SaveSlotMenu : MonoBehaviour
{
    [Header("UI 元素")]
    public TMP_Text titleText; 
    public GameObject[] saveSlots; // 每个 slot 按钮
    public TMP_Text[] levelTexts;
    public TMP_Text[] fragmentTexts;
    public TMP_Text[] timeTexts;
    public Button backButton;

    [Header("主菜单引用")]
    public GameObject mainMenuPanel; // 返回主菜单用

    [Header("关卡映射（可选）")]
    public string[] levelSceneNames; // 如果 Build Index 不连续，用场景名字映射

    private void Start()
    {
        if (titleText != null)
            titleText.text = "读取存档";

        // 初始化存档槽
        for (int i = 0; i < saveSlots.Length; i++)
        {
            int index = i;
            Button btn = saveSlots[i].GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnSlotClicked(index));

            LoadSlotInfo(index);
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(OnBackClicked);
        }
    }

    private void LoadSlotInfo(int index)
    {
        SaveData data = SaveSystem.Load(index); // 读取存档

        if (data != null)
        {
            levelTexts[index].text = $"关卡：{data.level}";
            fragmentTexts[index].text = $"碎片：{data.fragmentCount}";
            timeTexts[index].text = $"时间：{data.saveTime}";
        }
        else
        {
            levelTexts[index].text = "空存档";
            fragmentTexts[index].text = "";
            timeTexts[index].text = "";
        }
    }

    private void OnSlotClicked(int index)
    {
        SaveData data = SaveSystem.Load(index);

        if (data == null)
        {
            Debug.Log($"⚠️ 存档槽 {index + 1} 为空。");
            return;
        }

        // --- 使用自定义场景映射 ---
        if (levelSceneNames != null && levelSceneNames.Length > 0)
        {
            int nextLevelNumber = data.level + 1; // 下一关关卡编号
            if (nextLevelNumber - 1 < levelSceneNames.Length)
            {
                string sceneName = levelSceneNames[nextLevelNumber - 1];
                Debug.Log($"加载存档 {index + 1}，进入场景：{sceneName}");
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogWarning("⚠️ 已经是最后一个关卡，返回主菜单");
                SceneManager.LoadScene("Menu");
            }
        }
        else // --- 默认使用 Build Index 跳转 ---
        {
            int nextBuildIndex = data.level + 1;
            if (nextBuildIndex < SceneManager.sceneCountInBuildSettings)
            {
                Debug.Log($"加载存档 {index + 1}，进入关卡 Build Index：{nextBuildIndex}");
                SceneManager.LoadScene(nextBuildIndex);
            }
            else
            {
                Debug.LogWarning("⚠️ 已经是最后一个关卡，返回主菜单");
                SceneManager.LoadScene("Menu");
            }
        }
    }

    private void OnBackClicked()
    {
        gameObject.SetActive(false);
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
    }

    /// <summary>
    /// 检查是否存在存档，返回按钮可以使用
    /// </summary>
    public bool HasAnySave()
    {
        for (int i = 0; i < saveSlots.Length; i++)
        {
            if (SaveSystem.Load(i) != null)
                return true;
        }
        return false;
    }
}
