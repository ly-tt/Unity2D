using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SaveSlotMenu : MonoBehaviour
{
    [Header("存档槽按钮（3 个）")]
    [SerializeField] private Button[] saveSlotButtons; // 对应 SaveSlot_1, 2, 3

    [Header("返回按钮")]
    [SerializeField] private Button backButton;

    [Header("游戏场景名称")]
    [SerializeField] private string gameSceneName = "GameScene";

    private void Start()
    {
        // --- 防御性检查 ---
        if (saveSlotButtons == null || saveSlotButtons.Length == 0)
        {
            Debug.LogError("⚠️ SaveSlotButtons 未绑定，请在 Inspector 中设置！");
            return;
        }

        // --- 为每个存档按钮绑定事件 ---
        for (int i = 0; i < saveSlotButtons.Length; i++)
        {
            if (saveSlotButtons[i] == null)
            {
                Debug.LogError($"⚠️ SaveSlotButtons[{i}] 未绑定！");
                continue;
            }

            int slotIndex = i; // 必须缓存局部变量，避免闭包问题
            saveSlotButtons[i].onClick.AddListener(() => OnSaveSlotClicked(slotIndex));
        }

        // --- 返回按钮 ---
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackClicked);
        }
        else
        {
            Debug.LogError("⚠️ BackButton 未绑定！");
        }
    }

    // 点击某个存档槽
    private void OnSaveSlotClicked(int index)
    {
        Debug.Log($"📂 点击了第 {index + 1} 个存档槽");

        // 保存当前选择的槽位（可以在游戏内使用）
        PlayerPrefs.SetInt("SelectedSaveSlot", index + 1);
        PlayerPrefs.Save();

        // TODO: 检查该槽位是否有存档数据
        // 暂时直接加载游戏场景
        SceneManager.LoadScene(gameSceneName);
    }

    // 返回按钮逻辑
    private void OnBackClicked()
    {
        Debug.Log("↩ 返回主菜单");

        // 找到主菜单对象并显示
        MainMenu mainMenu = FindObjectOfType<MainMenu>();
        if (mainMenu != null)
        {
            mainMenu.ShowMainMenu();
        }
        else
        {
            Debug.LogWarning("⚠️ 未找到 MainMenu 实例！");
        }

        // 隐藏当前面板
        gameObject.SetActive(false);
    }
}
