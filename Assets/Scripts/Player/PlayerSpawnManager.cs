using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    [Header("玩家出生点")]
    [SerializeField] private Transform respawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
            Debug.Log($"玩家已出生在复活点：{respawnPoint.position}");
        }
        else
        {
            Debug.LogWarning("未指定 RespawnPoint，玩家将在原始位置生成。");
        }
    }
}
