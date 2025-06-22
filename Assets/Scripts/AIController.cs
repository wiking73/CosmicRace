using UnityEngine;
using System.Collections;

public class AIController : MonoBehaviour
{
    public Transform[] waypoints;
    public Transform[] overtakeWaypoints;
    public float maxSpeed = 20f;
    public float acceleration = 10f;
    public float deceleration = 20f;
    public float turnSpeed = 5f;
    // public float rayDistance = 7f;
    [SerializeField] private float rayDistance = 20f;
    [SerializeField] private LayerMask obstacleLayers;

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
    private float t = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (gameObject.name.Contains("Red"))
        {
            maxSpeed = 36f;
            acceleration = 25f;
            deceleration = 30f;
        }
        else if (gameObject.name.Contains("Yellow"))
        {
            maxSpeed = 38f;
            acceleration = 30f;
            deceleration = 30f;
        }
        else if (gameObject.name.Contains("Violet"))
        {
            maxSpeed = 34f;
            acceleration = 45f;
            deceleration = 30f;
        }

        targetSpeed = maxSpeed;
    }

    void FixedUpdate()
    {
        if (!GameManager.Instance.raceStarted || !aiActive)
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

    public IEnumerator BoostMassAndSpeed(float addedMass, float addedSpeed, float duration)
    {
        float originalSpeed = maxSpeed;
        float originalMass = rb.mass;

        maxSpeed += addedSpeed;
        targetSpeed = maxSpeed;
        rb.mass += addedMass;

        yield return new WaitForSeconds(duration);

        maxSpeed = originalSpeed;
        targetSpeed = maxSpeed;
        rb.mass = originalMass;

        Debug.Log(name + " zakończył boost.");
    }

    void MoveAlongWaypoints()
    {
        if (waypoints.Length < 4) return;

        if (currentWaypointIndex >= waypoints.Length - 1)
        {
            // Ostatni waypoint osiągnięty - zatrzymaj pojazd
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

        // Aktualizuj parametr t w zależności od prędkości i długości odcinka
        float segmentLength = Vector3.Distance(p1, p2);
        t += (currentSpeed * Time.fixedDeltaTime) / segmentLength;

        // Po osiągnięciu końca segmentu, przejdź do kolejnego
        if (t >= 1f)
        {
            t = 0f;
            currentWaypointIndex++;
            return;
        }

        // Wyznacz pozycj� na spline
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
        Vector3 rayStart = transform.position + Vector3.up * 0.5f;
        Vector3 rayDir = transform.forward;

        Debug.DrawRay(rayStart, rayDir * rayDistance, Color.red);

        if (Physics.Raycast(rayStart, rayDir, out hit, rayDistance, obstacleLayers))
        {
            if (hit.collider.gameObject == this.gameObject)
                return;

            Debug.Log(name + " hit: " + hit.collider.name);
            if (hit.collider.CompareTag("Player") || hit.collider.name.StartsWith("AI"))
            {
                Debug.Log(gameObject.name + " wykrył przeszkodę i rozpoczyna wyprzedzanie!");
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
            Debug.Log(gameObject.name + " zderzy� si� z graczem!");
            targetSpeed = maxSpeed * slowdownMultiplier;
            slowdownTimer = collisionSlowdownTime;

            Vector3 pushDir = (transform.position - collision.transform.position).normalized;
            rb.AddForce(pushDir * 300f);
        }
    }

    public void SetWaypoints(Transform[] newWaypoints, Transform[] newOvertakeWaypoints = null)
    {
        waypoints = newWaypoints;
        overtakeWaypoints = newOvertakeWaypoints ?? new Transform[0];
        currentWaypointIndex = 0;
        currentOvertakeIndex = 0;
        isOvertaking = false;

        Debug.Log(name + " zmienił trasę na nowe waypointy.");
    }
    public bool aiActive = true;
    public IEnumerator StopTemporarily(float duration)
    {
        bool originalState = aiActive; 
        aiActive = false;

        yield return new WaitForSeconds(duration);

        aiActive = originalState;
    }
}
