using UnityEngine;
using TMPro; // Pamiętaj o tym, jeśli używasz TextMeshPro

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Prompts")]
    [SerializeField] private GameObject flipCarPromptGameObject;
    [SerializeField] private TextMeshProUGUI temporaryMessageText;

    [Header("Custom Panel")]
    [SerializeField] private GameObject customPanelGameObject;

    private void Awake()
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

        if (flipCarPromptGameObject != null)
        {
            flipCarPromptGameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("UIManager: FlipCar Prompt Game Object is NOT assigned in the Inspector. Please assign it.", this);
        }

        if (temporaryMessageText != null)
        {
            temporaryMessageText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("UIManager: Temporary Message Text (TextMeshProUGUI) is NOT assigned in the Inspector. Temporary messages will not display.", this);
        }

        if (customPanelGameObject != null)
        {
            customPanelGameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("UIManager: Custom Panel Game Object is NOT assigned in the Inspector. Panel functionality will not work.", this);
        }
    }

    public void ShowFlipCarPrompt(bool show)
    {
        if (flipCarPromptGameObject != null)
        {
            flipCarPromptGameObject.SetActive(show);
        }
        else
        {
            Debug.LogWarning("UIManager: Attempted to show/hide FlipCar Prompt, but it's not assigned!", this);
        }
    }

    public void ShowTemporaryMessage(string message, bool show)
    {
        if (temporaryMessageText != null)
        {
            temporaryMessageText.text = message;
            temporaryMessageText.gameObject.SetActive(show);
        }
        else
        {
            Debug.LogWarning("UIManager: Attempted to show/hide temporary message, but Temporary Message Text (TextMeshProUGUI) is not assigned!", this);
        }
    }


    public void ShowCustomPanel(bool show)
    {
        if (customPanelGameObject != null)
        {
            customPanelGameObject.SetActive(show);
        }
        else
        {
            Debug.LogWarning("UIManager: Attempted to show/hide Custom Panel, but it's not assigned!", this);
        }
    }
}