using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string GetSavePath(int slotIndex)
    {
        return Path.Combine(Application.persistentDataPath, $"save_slot_{slotIndex}.json");
    }

    // 保存存档
    public static void Save(int slotIndex, SaveData data)
    {
        string path = GetSavePath(slotIndex);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log($"✅ 存档 {slotIndex + 1} 已保存到 {path}");
    }

    // 读取存档
    public static SaveData Load(int slotIndex)
    {
        string path = GetSavePath(slotIndex);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log($"✅ 成功加载存档 {slotIndex + 1}：关卡 {data.level}");
            return data;
        }
        else
        {
            Debug.LogWarning($"⚠️ 未找到存档文件：{path}");
            return null;
        }
    }

    // 判断是否存在存档
    public static bool SaveExists(int slotIndex)
    {
        return File.Exists(GetSavePath(slotIndex));
    }

    // 删除存档（可选）
    public static void Delete(int slotIndex)
    {
        string path = GetSavePath(slotIndex);
        if (File.Exists(path))
            File.Delete(path);
    }
}