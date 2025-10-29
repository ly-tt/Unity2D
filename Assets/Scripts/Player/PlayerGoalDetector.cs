using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGoalDetector : MonoBehaviour
{
    [Header("终点检测对象")]
    public Collider2D targetSquare;

    [Header("通关面板")]
    public GameObject levelCompletePanel;

    [Header("门的图片")]
    public GameObject closedDoorImage; // 原本显示的关门图片
    public GameObject openDoorImage;   // 通关后显示的开门图片

    private bool hasCompleted = false;

    private void OnTriggerEnter2D(Collider2D other)  
    {
        if (hasCompleted) return;

        Debug.Log("碰到: " + other.name); // 调试用

        if (other == targetSquare)
        {
            Debug.Log("玩家到达终点！");
            hasCompleted = true;

            // 显示通关面板
            if (levelCompletePanel != null)
            {
                levelCompletePanel.SetActive(true);
                Time.timeScale = 0f;
                Debug.Log("通关面板已激活");
            }
            else
            {
                Debug.LogError("通关面板未赋值！");
            }

            // 替换门的图片
            if (closedDoorImage != null && openDoorImage != null)
            {
                closedDoorImage.SetActive(false); // 隐藏关门
                openDoorImage.SetActive(true);    // 显示开门
                Debug.Log("门已打开，图片已切换");
            }
            else
            {
                Debug.LogError("门图片未赋值！");
            }
        }
    }
}
