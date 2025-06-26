using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Position : MonoBehaviour
{
    public List<Transform> allCars;
    public TextMeshProUGUI[] positionTexts;
    public List<Transform> waypoints;

    void Update()
    {
        if (allCars == null || allCars.Count == 0) return;
        if (positionTexts == null || positionTexts.Length == 0) return;
        if (waypoints == null || waypoints.Count == 0) return;

        allCars = allCars.Where(car => car != null).ToList();

        foreach (Transform car in allCars)
        {
            CarProgress progress = car.GetComponent<CarProgress>();
            if (progress != null)
            {
                // ZnajdŸ najbli¿szy waypoint
                float minDist = float.MaxValue;
                int closestIndex = 0;

                for (int i = 0; i < waypoints.Count; i++)
                {
                    float dist = Vector3.Distance(car.position, waypoints[i].position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closestIndex = i;
                    }
                }

                progress.currentWaypointIndex = closestIndex;
            }
        }


        // Sortowanie — najpierw po numerze waypointu, potem po odleg³oœci do tego waypointa
        allCars.Sort((a, b) =>
        {
            if (a == null)
            {
                Debug.LogWarning(a.name + "jest nullem");
            }
            if (b == null)
            {
                Debug.LogWarning(b.name + "jest nullem");
            }
            var aProg = a.GetComponent<CarProgress>();
            var bProg = b.GetComponent<CarProgress>();

            int cmp = bProg.currentWaypointIndex.CompareTo(aProg.currentWaypointIndex);
            if (cmp != 0) return cmp;

            // Jeœli ten sam waypoint — kto bli¿ej nastêpnego
            float distA = Vector3.Distance(a.position, waypoints[aProg.currentWaypointIndex].position);
            float distB = Vector3.Distance(b.position, waypoints[bProg.currentWaypointIndex].position);
            return distA.CompareTo(distB);
        });

        // Wyœwietlenie pozycji
        int count = Mathf.Min(positionTexts.Length, allCars.Count);
        for (int i = 0; i < count; i++)
        {
            if (allCars[i] == null) continue;

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
