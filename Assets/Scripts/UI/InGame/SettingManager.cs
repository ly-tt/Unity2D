using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingManager : MonoBehaviour
{
    [Header("音量滑条")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;

    [Header("按钮")]
    [SerializeField] private Button backButton;

    private InGameUIManager uiManager;

    private void Start()
    {
        uiManager = FindObjectOfType<InGameUIManager>();

        // 初始化滑条值
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        bgmVolumeSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);

        // 注册事件
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        backButton.onClick.AddListener(OnBackClicked);
    }

    private void OnMasterVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
        AudioListener.volume = value; // 如果没有 Mixer，就直接调全局音量
    }

    private void OnBGMVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("BGMVolume", value);
        PlayerPrefs.Save();
    }

    private void OnBackClicked()
    {
        gameObject.SetActive(false);
        if (uiManager != null)
        {
            uiManager.pauseMenuPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("未找到 InGameUIManager！");
        }
    }
}
