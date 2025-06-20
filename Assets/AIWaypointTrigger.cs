using UnityEngine;

public class AIWaypointTrigger : MonoBehaviour
{
    [SerializeField] private Transform[] newWaypoints;
    [SerializeField] private Transform[] newOvertakeWaypoints;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AI"))
        {
            AIController ai = other.GetComponent<AIController>();
            if (ai != null)
            {
                ai.SetWaypoints(newWaypoints, newOvertakeWaypoints);
            }
        }
    }
}
