using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlotManager : MonoBehaviour
{
    public GameObject[] saveSlots; // Inspector绑定三个存档slot
    public TextMeshProUGUI[] slotInfoTexts;

    private SaveSlotMode currentMode;
    private InGameUIManager uiManager;

    private void Awake()
    {
        uiManager = FindObjectOfType<InGameUIManager>();
    }

    public void Init(SaveSlotMode mode)
    {
        currentMode = mode;
        for(int i = 0; i < saveSlots.Length; i++)
        {
            int index = i;
            Button btn = saveSlots[i].GetComponentInChildren<Button>();
            btn.onClick.RemoveAllListeners();

            if(mode == SaveSlotMode.Save)
                btn.onClick.AddListener(() => SaveSlot(index));
            else
                btn.onClick.AddListener(() => LoadSlot(index));
        }
    }

    private void SaveSlot(int index)
    {
        Debug.Log($"保存到存档 {index+1}");
        // TODO: 写入实际存档数据
    }

    private void LoadSlot(int index)
    {
        Debug.Log($"读取存档 {index+1}");
        // TODO: 加载存档数据
    }
}
