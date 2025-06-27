using UnityEngine;
using UnityEngine.UI;

public class ToggleSFXButton : MonoBehaviour
{
    public Button toggleButton;
    public Image buttonIcon;
    public Sprite sfxOnIcon;
    public Sprite sfxOffIcon;

    void Start()
    {
        if (toggleButton == null)
        {
            toggleButton = GetComponent<Button>();
        }
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleSFXState);
        }
        else
        {
            Debug.LogError("ToggleSFXButton: Toggle Button component is null on Start! Make sure it's assigned or on the same GameObject.", this);
            enabled = false;
            return;
        }
        if (SFXManager.Instance == null)
        {
            Debug.LogError("ToggleSFXButton: SFXManager.Instance is null. Ensure SFXManager is in your scene and has a higher Script Execution Order.", this);
            enabled = false;
            return;
        }
        
        if (buttonIcon == null || sfxOnIcon == null || sfxOffIcon == null)
        {
            Debug.LogError("ToggleSFXButton: Button Icon Image or Sprites not assigned in the Inspector!", this);
            enabled = false;
            return;
        }

        UpdateButtonIcon();
    }

    void ToggleSFXState()
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.ToggleSFXSound();
            UpdateButtonIcon();
        }
        else
        {
            Debug.LogError("ToggleSFXButton: Cannot toggle SFX sound, SFXManager.Instance is null when clicking button.", this);
        }
    }

    void UpdateButtonIcon()
    {
        if (SFXManager.Instance != null && buttonIcon != null)
        {
            buttonIcon.sprite = SFXManager.Instance.IsSFXMuted() ? sfxOffIcon : sfxOnIcon;
        }
    }
}