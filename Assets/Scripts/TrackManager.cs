using UnityEngine;
using System.Linq; 

public class TrackManager : MonoBehaviour
{
    public static TrackManager Instance { get; private set; }

    private TrackWaypointContainer currentWaypointContainer;
    [SerializeField] private Transform defaultFallbackRespawnPoint;
    public Transform[] allRespawnPoints; 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("TrackManager: Instancja utworzona i ustawiona.", this);
        }
        else
        {
            Debug.LogWarning("TrackManager: Wykryto duplikat. Niszczę ten obiekt, aby utrzymać pojedynczą instancję.", this);
            Destroy(gameObject);
        }
    }

    public void SetActiveRespawnPoints(TrackWaypointContainer newContainer)
    {
        if (newContainer != null && newContainer != currentWaypointContainer)
        {
            currentWaypointContainer = newContainer;
            Debug.Log("TrackManager: Zaktualizowano aktywny kontener punktów respawnu na: " + newContainer.name, newContainer);
        }
    }

    public Transform GetNearestRespawnPoint(Vector3 referencePosition)
    {
        Transform bestPoint = null;

        if (currentWaypointContainer != null && 
            currentWaypointContainer.respawnPoints != null && 
            currentWaypointContainer.respawnPoints.Length > 0)
        {
            bestPoint = currentWaypointContainer.GetNearestPoint(referencePosition);
            if (bestPoint != null)
            {
                return bestPoint ;
            }
            else
            {
                Debug.LogWarning("TrackManager: Aktywny kontener (" + currentWaypointContainer.name + ") nie zwrócił najbliższego punktu (może nie ma punktów?). Używam fallback point, jeśli jest dostępny.", this);
            }
        }
        else
        {
            Debug.LogWarning("TrackManager: currentWaypointContainer jest NULL! Upewnij się, że samochód wjechał w TrackChecker, który ustawił kontener punktów respawnu. Używam fallback point, jeśli jest dostępny.", this);
        }

        if (allRespawnPoints != null && allRespawnPoints.Length > 0)
        {
            float minDist = float.MaxValue;
            Transform nearestGlobal = null;

            foreach (var point in allRespawnPoints)
            {
                if (point == null) continue;

                float dist = Vector3.Distance(referencePosition, point.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearestGlobal = point;
                }
            }

            if (nearestGlobal != null)
            {
                Debug.Log("TrackManager: Znaleziono najbliższy punkt z globalnej listy.", this);
                return nearestGlobal;
            }
            else
            {
                Debug.LogWarning("TrackManager: Globalna lista punktów respawnu jest pusta lub zawiera same NULL-e!", this);
            }
        }

        if (defaultFallbackRespawnPoint != null)
        {
            return defaultFallbackRespawnPoint;
        }

        Debug.LogError("TrackManager: Brak jakichkolwiek dostępnych punktów respawnu (ani aktywnego kontenera, ani domyślnego fallback point)!", this);
        return null;
    }
}