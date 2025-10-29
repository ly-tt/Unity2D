using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("音量滑条")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("设置菜单按钮")]
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button backButton;

    [Header("引用")]
    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private GameObject controlsPanel;

    private void Start()
    {
        // 如果未在Inspector中赋值，尝试自动查找
        if (!mainMenu) mainMenu = FindObjectOfType<MainMenu>();
        if (!controlsPanel) controlsPanel = GameObject.Find("ControlsPanel");

        // 初始化滑条值
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        bgmVolumeSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // 注册事件
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        
        controlsButton.onClick.AddListener(OnControlsClicked);
        backButton.onClick.AddListener(OnBackClicked);
    }

    private void OnMasterVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
        AudioListener.volume = value;
    }

    private void OnBGMVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("BGMVolume", value);
        PlayerPrefs.Save();
        // 这里可以添加BGM音量控制的逻辑
    }

    private void OnSFXVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
        // 这里可以添加SFX音量控制的逻辑
    }

    private void OnControlsClicked()
    {
        Debug.Log("打开控制说明");
        if (controlsPanel != null && mainMenu != null)
        {
            mainMenu.ShowControlsMenu();
        }
    }

    private void OnBackClicked()
    {
        Debug.Log("返回主菜单");
        if (mainMenu != null)
        {
            mainMenu.ShowMainMenu();
        }
    }
}