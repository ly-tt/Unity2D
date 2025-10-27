using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button mainMenuButton;

    private InGameUIManager uiManager;

    private void Start()
    {
        uiManager = FindObjectOfType<InGameUIManager>();

        // 安全检查
        if (uiManager == null)
        {
            Debug.LogError("未找到 InGameUIManager！");
            return;
        }

        // 为按钮添加安全检查
        if (resumeButton != null)
            resumeButton.onClick.AddListener(() => uiManager.ResumeGame());
        else
            Debug.LogError("ResumeButton 未赋值！");

        if (settingsButton != null)
            settingsButton.onClick.AddListener(() => uiManager.OpenSettings());
        else
            Debug.LogError("SettingsButton 未赋值！");

        if (controlsButton != null)
            controlsButton.onClick.AddListener(() => uiManager.OpenControls());
        else
            Debug.LogError("ControlsButton 未赋值！");

        if (saveButton != null)
            saveButton.onClick.AddListener(() => uiManager.OpenSaveMenu(SaveSlotMode.Load));
        else
            Debug.LogError("SaveButton 未赋值！");

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(() => 
                uiManager.ShowConfirmation("确定返回主菜单？", uiManager.ReturnToMenu)
            );
        else
            Debug.LogError("MainMenuButton 未赋值！");
    }
}