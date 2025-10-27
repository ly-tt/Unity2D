using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManger : MonoBehaviour
{
    public static PauseManger Instance;

    public GameObject pauseMenuPanel;
    private bool isPaused = false;

    void Awake()
    {
        Instance = this;
        pauseMenuPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

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
}