using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsMenu : MonoBehaviour
{
    [SerializeField] private Button backButton;

    private InGameUIManager uiManager;

    private void Start()
    {
        uiManager = FindObjectOfType<InGameUIManager>();
        backButton.onClick.AddListener(OnBack);
    }

    private void OnBack()
    {
        gameObject.SetActive(false);
        if (uiManager != null)
        {
            uiManager.pauseMenuPanel.SetActive(true);
        }
    }
}
