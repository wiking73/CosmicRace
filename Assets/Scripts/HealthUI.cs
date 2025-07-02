using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthUI : MonoBehaviour
{
    public TextMeshProUGUI hpText;
    public Image hpFillImage;

    public Color highColor = Color.green;
    public Color midColor = Color.yellow;
    public Color lowColor = Color.red;

    public void UpdateHP(float current, float max)
    {
        hpText.text = "HP: " + Mathf.RoundToInt(current);

        float percent = current / max;
        hpFillImage.fillAmount = percent;

        if (percent > 0.6f)
            hpFillImage.color = highColor;
        else if (percent > 0.3f)
            hpFillImage.color = midColor;
        else
            hpFillImage.color = lowColor;
    }
}
