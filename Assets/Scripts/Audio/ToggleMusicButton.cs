using UnityEngine;
using UnityEngine.UI;

public class ToggleMusicButton : MonoBehaviour
{
    public Button toggleButton;
    public Text buttonText;

    void Start()
    {
        toggleButton.onClick.AddListener(ToggleMusicIcon);
        UpdateButtonText();
    }

    void ToggleMusicIcon()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.ToggleMusic();
            UpdateButtonText();
        }
    }

    void UpdateButtonText()
    {
        if (MusicManager.Instance != null)
        {
            buttonText.text = MusicManager.Instance.IsMuted() ? "unmute" : "mute";
        }
    }
}
