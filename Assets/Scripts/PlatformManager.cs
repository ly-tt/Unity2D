// PlatformManager.cs
using System.Collections;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    private static PlatformManager instance;
    public static PlatformManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("PlatformManager");
                instance = obj.AddComponent<PlatformManager>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void ScheduleRespawn(GameObject platform, float delay)
    {
        StartCoroutine(RespawnAfterDelay(platform, delay));
    }

    private IEnumerator RespawnAfterDelay(GameObject platform, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (platform != null)
        {
            platform.SetActive(true);
            var comp = platform.GetComponent<DisappearingPlatform>();
            if (comp != null)
            {
                comp.ResetState();
            }
        }
    }
}