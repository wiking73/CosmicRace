using UnityEngine;
using TMPro;

public class HeassssslthUI : MonoBehaviour
{
    public TextMeshProUGUI hpText;

    public void UpdateeHP(float current, float max)
    {
        hpText.text = "HP: " + Mathf.RoundToInt(current) + " / " + Mathf.RoundToInt(max);
    }
}
