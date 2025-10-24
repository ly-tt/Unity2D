using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("音量滑条")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;

    [Header("按钮")]
    [SerializeField] private Button backButton;

    // 音频混音器（可选，如果你使用 Unity Mixer）
    // [SerializeField] private AudioMixer audioMixer;

    private void Start()
    {
        // 初始化滑条值
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        bgmVolumeSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);

        // 注册监听
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        backButton.onClick.AddListener(OnBackClicked);
    }

    private void OnMasterVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();

        // 如果有 Mixer，这里可以控制全局音量：
        // audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);

        Debug.Log($"主音量调整为 {value}");
    }

    private void OnBGMVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("BGMVolume", value);
        PlayerPrefs.Save();

        Debug.Log($"背景音乐音量调整为 {value}");
    }

    private void OnBackClicked()
    {
        // 找回主菜单对象
        MainMenu menu = FindObjectOfType<MainMenu>();
        if (menu != null)
            menu.ShowMainMenu();
        else
            Debug.LogWarning("未找到 MainMenu 实例！");

        gameObject.SetActive(false); // 隐藏当前菜单
    }
}