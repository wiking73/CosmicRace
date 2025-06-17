using UnityEngine;
using UnityEngine.UI;

public class ToggleUISoundButton : MonoBehaviour
{
    public Button toggleButton;
    public Text buttonText;

    void Start()
    {
        Debug.Log("ToggleUISoundButton.Start() — toggleButton=" + toggleButton + ", buttonText=" + buttonText);

        if (toggleButton == null)
        {
            toggleButton = GetComponent<Button>();
        }
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleUISound);
        }

        if (UIButtonSoundManager.Instance == null)
        {
            Debug.LogError("UIButtonSoundManager.Instance is null. Make sure UISoundManager is in your scene and set up correctly.");
            return;
        }

        UpdateButtonText();
    }

    void ToggleUISound()
    {
        Debug.Log("ToggleUISound() called! isUIMuted=" + UIButtonSoundManager.Instance.IsUIMuted());

        if (UIButtonSoundManager.Instance != null)
        {
            UIButtonSoundManager.Instance.ToggleUIMute();
            UpdateButtonText();
        }
    }

    void UpdateButtonText()
    {
        Debug.Log("UpdateButtonText() — setting text to " + (UIButtonSoundManager.Instance.IsUIMuted() ? "UNMUT" : "MUT"));

        if (UIButtonSoundManager.Instance != null)
        {
            if (buttonText != null)
            {
                buttonText.text = UIButtonSoundManager.Instance.IsUIMuted() ? "UNMUT" : "MUT";
            }
        }
    }
    
    public void DebugClick()
{
    Debug.Log("Przycisk kliknięty!");
}

}