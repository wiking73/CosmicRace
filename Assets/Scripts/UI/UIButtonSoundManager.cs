using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; 

public class UIButtonSoundManager : MonoBehaviour 
{
    public static UIButtonSoundManager Instance { get; private set; }

    public AudioClip clickSound;
    public AudioClip hoverSound;
    public string buttonTag = "UIButtonSound";

    private AudioSource audioSource;
    private bool isUIMuted = false;

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

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
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

    public void ToggleUIMute()
    {
        isUIMuted = !isUIMuted;
        audioSource.mute = isUIMuted;
        Debug.Log("UI Sounds Muted: " + isUIMuted);
    }

    public bool IsUIMuted()
    {
        return isUIMuted;
    }

    public void PlayClickSound()
    {
        if (clickSound != null && audioSource != null && !isUIMuted) 
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    public void PlayHoverSound()
    {
        if (hoverSound != null && audioSource != null && !isUIMuted) 
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }
}