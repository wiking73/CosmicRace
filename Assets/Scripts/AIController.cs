using UnityEngine;

public class AIController : MonoBehaviour
{
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;

    public float speed = 10f;
    public float turnSpeed = 5f;
    public float waypointThreshold = 3f;

    private void Update()
    {
        MoveTowardsWaypoint();
    }

    void MoveTowardsWaypoint()
    {
        if (waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;

        // Obrót w kierunku waypointa
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, turnSpeed * Time.deltaTime);

        // Ruch do przodu
        transform.position += transform.forward * speed * Time.deltaTime;

        // SprawdŸ czy dotar³ do waypointa
        if (Vector3.Distance(transform.position, targetWaypoint.position) < waypointThreshold)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }
}
