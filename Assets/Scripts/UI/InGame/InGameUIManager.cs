using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System;

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

    [Header("通用UI元素")]
    public TextMeshProUGUI messageText; // 确认框
    public Button confirmButton;
    public Button cancelButton;

    [Header("LevelComplete按钮")]
    public Button nextLevelButton;
    public Button restartLevelButton;

    private bool isPaused = false;

    private void Start()
    {
        HideAllPanels();
        hudPanel.SetActive(true);

        if(nextLevelButton != null)
            nextLevelButton.onClick.AddListener(() => {
                Time.timeScale = 1f;
                LoadNextLevel();
            });

        if(restartLevelButton != null)
            restartLevelButton.onClick.AddListener(() => {
                Time.timeScale = 1f;
                RestartLevel();
            });
    }

    public void HideAllPanels()
    {
        hudPanel.SetActive(false);
        levelCompletePanel.SetActive(false);
        deathPanel.SetActive(false);
        pauseMenuPanel.SetActive(false);
        saveSlotMenuPanel.SetActive(false);
        settingsMenuPanel.SetActive(false);
        controlsMenuPanel.SetActive(false);
        confirmationPanel.SetActive(false);
    }

    // ===== 通关 / 死亡 =====
    public void ShowLevelComplete()
    {
        HideAllPanels();
        levelCompletePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ShowDeathPanel()
    {
        HideAllPanels();
        deathPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // ===== 暂停 =====
    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    // ===== 面板跳转 =====
    public void OpenSettings()
    {
        pauseMenuPanel.SetActive(false);
        settingsMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OpenControls()
    {
        pauseMenuPanel.SetActive(false);
        controlsMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OpenSaveMenu(SaveSlotMode mode)
    {
        saveSlotMenuPanel.SetActive(true);
        saveSlotMenuPanel.GetComponent<SaveSlotManager>().Init(mode);
    }

    // ===== 确认框 =====
    public void ShowConfirmation(string message, Action onConfirm)
    {
        confirmationPanel.SetActive(true);
        messageText.text = message;

        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        confirmButton.onClick.AddListener(() => {
            confirmationPanel.SetActive(false);
            onConfirm?.Invoke();
        });

        cancelButton.onClick.AddListener(() => {
            confirmationPanel.SetActive(false);
        });
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
        if(next < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(next);
        else
            ReturnToMenu();
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
