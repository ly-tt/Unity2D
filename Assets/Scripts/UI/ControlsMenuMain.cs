using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsMenuMain : MonoBehaviour
{
    [Header("控制说明按钮")]
    [SerializeField] private Button backButton;

    [Header("引用")]
    [SerializeField] private MainMenu mainMenu;

    private void Start()
    {
        // 如果未在Inspector中赋值，尝试自动查找
        if (!mainMenu) mainMenu = FindObjectOfType<MainMenu>();

        // 注册事件
        backButton.onClick.AddListener(OnBackClicked);
    }

    private void OnBackClicked()
    {
        Debug.Log("从控制说明返回设置");
        if (mainMenu != null)
        {
            mainMenu.ShowSettingsMenu();
        }
    }
}
