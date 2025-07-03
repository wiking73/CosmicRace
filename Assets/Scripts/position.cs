using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class Position : MonoBehaviour
{
    public List<Transform> allCars;
    public TextMeshProUGUI[] positionTexts;
    public List<Transform> waypoints;

    void Update()
    {
        // podstawowe weryfikacje
        if (allCars == null || allCars.Count == 0) return;
        if (positionTexts == null || positionTexts.Length == 0) return;
        if (waypoints == null || waypoints.Count == 0) return;
        if (RaceManager.Instance == null) return;

        // usuwamy z listy nullowe pozycje
        allCars = allCars.Where(c => c != null).ToList();

        // najpierw obliczamy postêp (closest waypoint) wszystkich wci¹¿ œcigaj¹cych siê
        foreach (var car in allCars)
        {
            var prog = car.GetComponent<CarProgress>();
            if (prog == null) continue;

            float minDist = float.MaxValue;
            int closest = 0;
            for (int i = 0; i < waypoints.Count; i++)
            {
                float d = Vector3.Distance(car.position, waypoints[i].position);
                if (d < minDist)
                {
                    minDist = d;
                    closest = i;
                }
            }
            prog.currentWaypointIndex = closest;
        }

        // rozdzielamy listê na finished i racing
        var finishedNames = RaceManager.Instance.finishOrder
            .Select(fr => fr.racerName).ToList();

        var finishedCars = new List<Transform>();
        var racingCars = new List<Transform>();
        foreach (var car in allCars)
        {
            if (finishedNames.Contains(car.name))
                finishedCars.Add(car);
            else
                racingCars.Add(car);
        }

        // 1) Sortujemy finishedCars wed³ug kolejnoœci w finishOrder
        finishedCars = finishedCars
            .OrderBy(c => finishedNames.IndexOf(c.name))
            .ToList();

        // 2) Sortujemy racingCars po waypoint + odleg³oœæ
        racingCars.Sort((a, b) =>
        {
            var pa = a.GetComponent<CarProgress>();
            var pb = b.GetComponent<CarProgress>();

            int cmp = pb.currentWaypointIndex.CompareTo(pa.currentWaypointIndex);
            if (cmp != 0) return cmp;

            float da = Vector3.Distance(a.position, waypoints[pa.currentWaypointIndex].position);
            float db = Vector3.Distance(b.position, waypoints[pb.currentWaypointIndex].position);
            return da.CompareTo(db);
        });

        // 3) Sklejamy w jedn¹ listê
        var finalOrder = finishedCars.Concat(racingCars).ToList();

        // 4) Wyœwietlamy w UI
        int count = Mathf.Min(positionTexts.Length, finalOrder.Count);
        for (int i = 0; i < count; i++)
        {
            var car = finalOrder[i];
            if (car == null) continue;

            string name = car.name;
            if (name.StartsWith("Player"))
                positionTexts[i].text = $"{i + 1}. <u><color=red>{name}</color></u>";
            else
                positionTexts[i].text = $"{i + 1}. {name}";
        }
    }
}

