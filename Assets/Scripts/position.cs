using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Position : MonoBehaviour
{
    public List<Transform> allCars; 
    public TextMeshProUGUI[] positionTexts; 

    void Update()
    {
        if (allCars == null || allCars.Count == 0) return;
        if (positionTexts == null || positionTexts.Length == 0) return;

        allCars.Sort((a, b) => a.position.z.CompareTo(b.position.z));


        int count = Mathf.Min(positionTexts.Length, allCars.Count);
        for (int i = 0; i < count; i++)
        {
            string carName = allCars[i].name;

            
            if (carName.StartsWith("Player"))
            {
                positionTexts[i].text = $"{i + 1}. <u><color=red>{carName}</color></u>";
            }
            else
            {
                positionTexts[i].text = $"{i + 1}. {carName}";
            }
        }
    }
}
