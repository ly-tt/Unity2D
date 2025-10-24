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

        resumeButton.onClick.AddListener(() => uiManager.ResumeGame());
        settingsButton.onClick.AddListener(() => uiManager.OpenSettings());
        controlsButton.onClick.AddListener(() => uiManager.OpenControls());
        saveButton.onClick.AddListener(() => uiManager.OpenSaveMenu(SaveSlotMode.Save));
        mainMenuButton.onClick.AddListener(() => 
            uiManager.ShowConfirmation("确定返回主菜单？", uiManager.ReturnToMenu)
        );
    }
}
