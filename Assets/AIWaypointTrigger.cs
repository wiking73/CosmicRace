using UnityEngine;

public class AIWaypointTrigger : MonoBehaviour
{
    public enum TriggerMode
    {
        ToSideRoad,
        ReturnToMain
    }

    [SerializeField] private TriggerMode triggerMode;

    [Header("Side Road")]
    [SerializeField] private Transform[] newWaypoints;
    [SerializeField] private Transform[] newOvertakeWaypoints;

    [Header("Return to Main Road")]
    [SerializeField] private Transform[] returnToMainWaypoints;
    [SerializeField] private Transform[] returnOvertakeWaypoints;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AI"))
        {
            AIController ai = other.GetComponent<AIController>();
            if (ai != null)
            {
                switch (triggerMode)
                {
                    case TriggerMode.ToSideRoad:
                        ai.SetWaypoints(newWaypoints, newOvertakeWaypoints);
                        break;

                    case TriggerMode.ReturnToMain:
                        ai.SetWaypoints(returnToMainWaypoints, returnOvertakeWaypoints);
                        break;
                }
            }
        }
    }
}
