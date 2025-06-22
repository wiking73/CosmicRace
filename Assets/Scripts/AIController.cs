using UnityEngine;

public class AIController : MonoBehaviour
{
    public Transform[] waypoints;
    public Transform[] overtakeWaypoints;
    public float maxSpeed = 20f;
    public float acceleration = 10f;
    public float deceleration = 20f;
    public float turnSpeed = 5f;
    public float rayDistance = 7f;
    public float collisionSlowdownTime = 2f;
    public float slowdownMultiplier = 0.5f;

    private int currentWaypointIndex = 0;
    private int currentOvertakeIndex = 0;
    private bool isOvertaking = false;
    private Rigidbody rb;
    private float targetSpeed = 0f;
    private float currentSpeed = 0f;
    private float slowdownTimer = 0f;
    public float lookAheadDistance = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (gameObject.name.Contains("Red"))
        {
            maxSpeed = 25f;
            acceleration = 12f;
            deceleration = 25f;
        }
        else if (gameObject.name.Contains("Yellow"))
        {
            maxSpeed = 22f;
            acceleration = 10f;
            deceleration = 22f;
        }
        else if (gameObject.name.Contains("Violet"))
        {
            maxSpeed = 20f;
            acceleration = 9f;
            deceleration = 20f;
        }

        targetSpeed = maxSpeed;
    }

    void FixedUpdate()
    {
        if (!GameManager.Instance.raceStarted)
            return;

        if (slowdownTimer > 0)
        {
            slowdownTimer -= Time.fixedDeltaTime;
            if (slowdownTimer <= 0)
            {
                targetSpeed = maxSpeed;
            }
        }

        UpdateSpeed();
        MoveAlongWaypoints();
        DetectObstacles();
    }

    void UpdateSpeed()
    {
        if (currentSpeed < targetSpeed)
        {
            currentSpeed += acceleration * Time.fixedDeltaTime;
            if (currentSpeed > targetSpeed)
                currentSpeed = targetSpeed;
        }
        else if (currentSpeed > targetSpeed)
        {
            currentSpeed -= deceleration * Time.fixedDeltaTime;
            if (currentSpeed < targetSpeed)
                currentSpeed = targetSpeed;
        }
    }

    private float t = 0f;

    void MoveAlongWaypoints()
    {
        if (waypoints.Length < 4) return;

        if (currentWaypointIndex >= waypoints.Length - 1)
        {
            // Ostatni waypoint osi¹gniêty — zatrzymaj pojazd
            currentSpeed = 0f;
            targetSpeed = 0f;
            return;
        }

        // Indeksy punktów dla spline
        int p0Index = Mathf.Clamp(currentWaypointIndex - 1, 0, waypoints.Length - 1);
        int p1Index = currentWaypointIndex;
        int p2Index = Mathf.Clamp(currentWaypointIndex + 1, 0, waypoints.Length - 1);
        int p3Index = Mathf.Clamp(currentWaypointIndex + 2, 0, waypoints.Length - 1);

        Vector3 p0 = waypoints[p0Index].position;
        Vector3 p1 = waypoints[p1Index].position;
        Vector3 p2 = waypoints[p2Index].position;
        Vector3 p3 = waypoints[p3Index].position;

        // Aktualizuj parametr t w zale¿noœci od prêdkoœci i d³ugoœci odcinka
        float segmentLength = Vector3.Distance(p1, p2);
        t += (currentSpeed * Time.fixedDeltaTime) / segmentLength;

        // Po osi¹gniêciu koñca segmentu, przejdŸ do kolejnego
        if (t >= 1f)
        {
            t = 0f;
            currentWaypointIndex++;
            return;
        }

        // Wyznacz pozycjê na spline
        Vector3 targetPos = GetCatmullRomPosition(t, p0, p1, p2, p3);

        // Obracanie w kierunku celu
        Vector3 targetDir = (targetPos - transform.position).normalized;
        targetDir.y = 0;

        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, turnSpeed * Time.fixedDeltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDir);

        // Ruch do przodu
        rb.MovePosition(transform.position + newDir * currentSpeed * Time.fixedDeltaTime);
    }

    // Catmull-Rom Spline helper
    Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }



    void DetectObstacles()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out hit, rayDistance))
        {
            if (hit.collider.CompareTag("Player") || hit.collider.name.StartsWith("AI"))
            {
                Debug.Log(gameObject.name + " wykry³ przeszkodê — rozpoczyna wyprzedzanie!");
                StartOvertaking();
            }
        }
    }

    void StartOvertaking()
    {
        if (!isOvertaking && overtakeWaypoints.Length > 0)
        {
            isOvertaking = true;
            currentOvertakeIndex = 0;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log(gameObject.name + " zderzy³ siê z graczem!");
            targetSpeed = maxSpeed * slowdownMultiplier;
            slowdownTimer = collisionSlowdownTime;

            Vector3 pushDir = (transform.position - collision.transform.position).normalized;
            rb.AddForce(pushDir * 300f);
        }
    }
}
