using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class SaveData
{
    public int level;           // 当前关卡编号
    public int fragmentCount;   // 收集碎片数
    public string saveTime;     // 保存时间字符串 (yyyy-MM-dd HH:mm:ss)

    public SaveData(int level, int fragmentCount)
    {
        this.level = level;
        this.fragmentCount = fragmentCount;
        this.saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
