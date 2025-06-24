using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [Header("UI Sounds")]
    public AudioClip clickSound;
    public AudioClip hoverSound;
    public string buttonTag = "UIButtonSound";

    private AudioSource sfxAudioSource;
    private bool isSFXMuted = false;
    
    private List<AudioSource> activeEngineAudioSources = new List<AudioSource>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        sfxAudioSource = GetComponent<AudioSource>();
        if (sfxAudioSource == null)
        {
            sfxAudioSource = gameObject.AddComponent<AudioSource>();
            sfxAudioSource.playOnAwake = false;
            sfxAudioSource.spatialBlend = 0;
            sfxAudioSource.volume = 1f;
        }

        sfxAudioSource.mute = isSFXMuted;
    }

    void Start()
    {
        SetupButtons();
    }

    public void SetupButtons()
    {
        GameObject[] taggedButtons = GameObject.FindGameObjectsWithTag(buttonTag);

        foreach (GameObject buttonGO in taggedButtons)
        {
            Button buttonComponent = buttonGO.GetComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.onClick.AddListener(() => PlayClickSound());
            }

            EventTrigger trigger = buttonGO.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = buttonGO.AddComponent<EventTrigger>();
            }
            trigger.triggers.Clear();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => { PlayHoverSound(); });
            trigger.triggers.Add(entry);
        }
    }

    public void RegisterEngineAudioSource(AudioSource source)
    {
        if (source != null && !activeEngineAudioSources.Contains(source))
        {
            activeEngineAudioSources.Add(source);
            source.mute = isSFXMuted;
        }
    }

     public void UnregisterEngineAudioSource(AudioSource source)
    {
        if (source != null && activeEngineAudioSources.Contains(source))
        {
            activeEngineAudioSources.Remove(source);
        }
    }

    public void ToggleSFXSound()
    {
        isSFXMuted = !isSFXMuted;
        sfxAudioSource.mute = isSFXMuted;
        
        foreach (AudioSource engineSource in activeEngineAudioSources)
        {
            if (engineSource != null)
            {
                engineSource.mute = isSFXMuted;
            }
        }
        Debug.Log("SFX (UI and Engine) Muted: " + isSFXMuted);
    }

    public bool IsSFXMuted()
    {
        return isSFXMuted;
    }

    public void PlayClickSound()
    {
        if (clickSound != null && sfxAudioSource != null && !isSFXMuted)
        {
            sfxAudioSource.PlayOneShot(clickSound);
        }
    }

    public void PlayHoverSound()
    {
        if (hoverSound != null && sfxAudioSource != null && !isSFXMuted)
        {
            sfxAudioSource.PlayOneShot(hoverSound);
        }
    }
    
    public void PlaySFX(AudioClip clip, float volume = 1.0f)
    {
        if (clip != null && sfxAudioSource != null && !isSFXMuted)
        {
            sfxAudioSource.PlayOneShot(clip, volume);
        }
    }
}