using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("音源")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("音量设置")]
    [Range(0, 1)] public float masterVolume = 1f;
    [Range(0, 1)] public float bgmVolume = 1f;
    [Range(0, 1)] public float sfxVolume = 1f;

    private AudioClip currentBGM;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadVolumeSettings();
    }

    private void LoadVolumeSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        ApplyVolume();
    }

    public void ApplyVolume()
    {
        if (bgmSource)
            bgmSource.volume = masterVolume * bgmVolume;
        if (sfxSource)
            sfxSource.volume = masterVolume * sfxVolume;
    }

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (clip == null || clip == currentBGM) return;
        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
        currentBGM = clip;
    }

    public void StopBGM()
    {
        bgmSource.Stop();
        currentBGM = null;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
    }
}
