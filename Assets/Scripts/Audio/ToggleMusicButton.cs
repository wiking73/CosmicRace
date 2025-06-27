using UnityEngine;
using UnityEngine.UI;

public class ToggleMusicButton : MonoBehaviour
{
    public Button toggleButton;
    public Image buttonIcon; 
    public Sprite musicOnIcon; 
    public Sprite musicOffIcon;


    void Start()
    {
        if (toggleButton == null)
        {
            toggleButton = GetComponent<Button>();
        }

        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleMusicState);
        }
        else
        {
            Debug.LogError("ToggleMusicButton: Toggle Button component is null on Start! Make sure it's assigned or on the same GameObject.", this);
            enabled = false;
            return;
        }

        if (buttonIcon == null || musicOnIcon == null || musicOffIcon == null)
        {
            Debug.LogError("ToggleMusicButton: Button Icon Image or Sprites not assigned in the Inspector!", this);
            enabled = false;
            return;
        }

        UpdateButtonIcon();
    }

    void ToggleMusicState()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.ToggleMusic();
            UpdateButtonIcon();
        }
        else
        {
            Debug.LogError("ToggleMusicButton: MusicManager.Instance is null when clicking button.", this);
        }
    }

    void UpdateButtonIcon()
    {
        if (MusicManager.Instance != null && buttonIcon != null)
        {
            buttonIcon.sprite = MusicManager.Instance.IsMuted() ? musicOffIcon : musicOnIcon;
        }
    }
}
