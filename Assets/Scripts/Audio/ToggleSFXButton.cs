using UnityEngine;
using UnityEngine.UI;

public class ToggleSFXButton : MonoBehaviour
{
    public Button toggleButton;
    public Text buttonText;

    void Start()
    {
        if (toggleButton == null)
        {
            toggleButton = GetComponent<Button>();
        }
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleSFXSound);
        }
        else
        {
            Debug.LogError("ToggleSFXButton: Toggle Button component is null on Start! Make sure it's assigned or on the same GameObject.", this);
            return;
        }
        if (SFXManager.Instance == null)
        {
            Debug.LogError("ToggleSFXButton: SFXManager.Instance is null. Ensure SFXManager is in your scene and has a higher Script Execution Order.", this);
        }

        UpdateButtonText();
    }

    void ToggleSFXSound()
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.ToggleSFXSound();
            UpdateButtonText();
        }
        else
        {
            Debug.LogError("ToggleSFXButton: Cannot toggle SFX sound, SFXManager.Instance is null when clicking button.", this);
        }
    }

    void UpdateButtonText()
    {
        if (SFXManager.Instance != null)
        {
            if (buttonText != null)
            {
                buttonText.text = SFXManager.Instance.IsSFXMuted() ? "UNMUTE" : "MUTE";
            }
            else
            {
                Debug.LogWarning("ToggleSFXButton: Button Text component is not assigned in the Inspector!", this);
            }
        }
    }
}