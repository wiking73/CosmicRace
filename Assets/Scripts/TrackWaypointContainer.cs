using UnityEngine;

public class TrackWaypointContainer : MonoBehaviour
{
    public Transform[] respawnPoints;
    public Transform GetNearestPoint(Vector3 referencePosition)
    {
        float minDist = float.MaxValue;
        Transform nearest = null;

        if (respawnPoints == null || respawnPoints.Length == 0)
        {
            Debug.LogWarning("TrackWaypointContainer: Brak punktów respawnu przypisanych do " + gameObject.name + ". Upewnij się, że tablica 'respawnPoints' jest wypełniona w Inspektorze.", this);
            return null;
        }

        foreach (var point in respawnPoints)
        {
            if (point == null) continue;

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