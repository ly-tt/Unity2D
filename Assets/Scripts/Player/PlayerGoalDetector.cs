using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGoalDetector : MonoBehaviour
{
    [Header("终点检测对象")]
    public Collider2D targetSquare;

    [Header("通关面板")]
    public GameObject levelCompletePanel;

    private bool hasCompleted = false;

    private void OnTriggerEnter2D(Collider2D other)  
    {
        if (hasCompleted) return;

        Debug.Log("碰到: " + other.name); // 调试用

        if (other == targetSquare)
        {
            Debug.Log("玩家到达终点！");
            hasCompleted = true;

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
        }
    }
}