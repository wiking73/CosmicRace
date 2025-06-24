using UnityEngine;

public class TrackChecker : MonoBehaviour
{
    [SerializeField] private Transform[] zoneRespawnPoints;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CarController car = other.GetComponent<CarController>();
            if (car != null)
            {
                car.SetRespawnPoints(zoneRespawnPoints);
                Debug.Log("Aktualizacja punktów respawnu na nowe (TrackChecker): " + name);
            }
        }
    }
    public Transform GetNearestPoint(Vector3 referencePosition)
    {
        float minDist = float.MaxValue;
        Transform nearest = null;

        foreach (var point in zoneRespawnPoints)
        {
            float dist = Vector3.Distance(referencePosition, point.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = point;
            }
        }

        return nearest;
    }

}
