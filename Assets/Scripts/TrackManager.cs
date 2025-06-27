using UnityEngine;

public class TrackManager : MonoBehaviour
{
    public static TrackManager Instance { get; private set; }

    private TrackWaypointContainer currentWaypointContainer;
    [SerializeField] private Transform defaultFallbackRespawnPoint;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
        if (currentWaypointContainer != null)
        {
            Transform point = currentWaypointContainer.GetNearestPoint(referencePosition);
            if (point != null)
            {
                return point;
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

        if (defaultFallbackRespawnPoint != null)
        {
            return defaultFallbackRespawnPoint;
        }

        Debug.LogError("TrackManager: Brak jakichkolwiek dostępnych punktów respawnu (ani aktywnego kontenera, ani domyślnego fallback point)!", this);
        return null;
    }
}