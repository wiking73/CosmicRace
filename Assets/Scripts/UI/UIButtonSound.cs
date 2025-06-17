using UnityEngine;
using UnityEngine.EventSystems; // Potrzebne do EventSystem
using UnityEngine.UI; // Potrzebne, jeśli chcemy np. wyłączyć interakcję na Buttonach

public class UIButtonSoundManager : MonoBehaviour // Zmieniamy nazwę na Manager
{
    public AudioClip clickSound;
    public AudioClip hoverSound;
    public string buttonTag = "UIButtonSound";

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void Start()
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

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => { PlayHoverSound(); });
            trigger.triggers.Add(entry);
        }
    }

    public void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    public void PlayHoverSound()
    {
        if (hoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }
}