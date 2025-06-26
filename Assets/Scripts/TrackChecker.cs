using UnityEngine;

public class TrackChecker : MonoBehaviour
{
    public TrackWaypointContainer trackWaypointContainer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (TrackManager.Instance != null && trackWaypointContainer != null)
            {
                TrackManager.Instance.SetActiveRespawnPoints(trackWaypointContainer);
                Debug.Log("TrackChecker: Zaktualizowano punkty respawnu w TrackManager na kontener: " + trackWaypointContainer.name, this);
            }
            else
            {
                if (TrackManager.Instance == null)
                    Debug.LogError("TrackChecker: TrackManager.Instance jest NULL! Upewnij się, że GameObject 'TrackManager' z komponentem 'TrackManager.cs' istnieje w scenie.", this);
                if (trackWaypointContainer == null)
                    Debug.LogError("TrackChecker: Pole 'Track Waypoint Container' nie jest przypisane w Inspektorze dla obiektu " + gameObject.name + "!", this);
            }
        }
    }
}