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
    public float maxDistanceFromWaypoint = 12f;
    public float waypointThresholdDistance = 4f;

    public AudioClip collisionSound;
    private AudioSource audioSource;

    public void TeleportToWaypoint(int waypointIndex)
    {
        if (waypointIndex < 0 || waypointIndex >= waypoints.Length)
        {
            Debug.LogWarning("Waypoint index out of range!");
            return;
        }

        currentWaypointIndex = waypointIndex;
        t = 0f;
        transform.position = waypoints[waypointIndex].position;
        transform.rotation = waypoints[waypointIndex].rotation;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Debug.Log(name + " został przesunięty na waypoint nr " + waypointIndex);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (gameObject.name.Contains("Red"))
        {
            maxSpeed = 38f;
            acceleration = 30f;
            deceleration = 30f;

        }
        else if (gameObject.name.Contains("Yellow"))
        {
            maxSpeed = 34f;
            acceleration = 25f;
            deceleration = 30f;
        }
        else if (gameObject.name.Contains("Violet"))
        {
            maxSpeed = 25f;
            acceleration = 25f;
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

    void MoveAlongWaypoints()
    {
        Transform[] currentPath = isOvertaking ? overtakeWaypoints : waypoints;
        int currentIndex = isOvertaking ? currentOvertakeIndex : currentWaypointIndex;

        // Jeżeli ostatni waypoint — zatrzymaj pojazd
        if (currentIndex >= currentPath.Length - (isOvertaking ? 0 : 1))
        {
            currentSpeed = 0f;
            targetSpeed = 0f;
            return;
        }

        if (isOvertaking)
        {
            // Ruch w trybie wyprzedzania — po prostych
            Vector3 targetPos = currentPath[currentIndex].position;
            Vector3 flatTargetDir = targetPos - transform.position;
            flatTargetDir.y = 0;

            float angleToTarget = Vector3.Angle(transform.forward, flatTargetDir.normalized);

            float speedFactor = Mathf.Clamp01(1f - (angleToTarget / 90f));
            targetSpeed = Mathf.Lerp(maxSpeed * 0.4f, maxSpeed, speedFactor);
            float dynamicTurnSpeed = Mathf.Lerp(turnSpeed * 0.3f, turnSpeed, speedFactor);

            Quaternion targetRotation = Quaternion.LookRotation(flatTargetDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, dynamicTurnSpeed * Time.deltaTime);

            rb.MovePosition(transform.position + transform.forward * currentSpeed * Time.fixedDeltaTime);

            float distance = flatTargetDir.magnitude;
            float dot = Vector3.Dot(transform.forward, flatTargetDir.normalized);

            if (distance < 3f || dot < 0f)
            {
                currentOvertakeIndex++;
                targetSpeed = maxSpeed;
            }
        }
        else
        {
            // Ruch po spline
            if (waypoints.Length < 4) return;

            int p0Index = Mathf.Clamp(currentWaypointIndex - 1, 0, waypoints.Length - 1);
            int p1Index = currentWaypointIndex;
            int p2Index = Mathf.Clamp(currentWaypointIndex + 1, 0, waypoints.Length - 1);
            int p3Index = Mathf.Clamp(currentWaypointIndex + 2, 0, waypoints.Length - 1);

            Vector3 p0 = waypoints[p0Index].position;
            Vector3 p1 = waypoints[p1Index].position;
            Vector3 p2 = waypoints[p2Index].position;
            Vector3 p3 = waypoints[p3Index].position;

            float segmentLength = Vector3.Distance(p1, p2);
            t += (currentSpeed * Time.fixedDeltaTime) / segmentLength;

            if (t >= 1f)
            {
                t = 0f;
                currentWaypointIndex++;
                return;
            }

            Vector3 targetPos = GetCatmullRomPosition(t, p0, p1, p2, p3);
            Vector3 targetDir = (targetPos - transform.position).normalized;
            targetDir.y = 0;

            float angleToTarget = Vector3.Angle(transform.forward, targetDir);
            if (angleToTarget > 60f)
            {
                targetSpeed = maxSpeed * 0.3f;  // bardzo ostry zakręt — mocno zwalniamy
            }
            else if (angleToTarget > 45f)
            {
                targetSpeed = maxSpeed * 0.5f;  // średnio ostry zakręt
            }
            float clampedAngle = Mathf.Clamp(angleToTarget, 0f, 60f);
            float speedFactor = Mathf.Clamp01(1f - (clampedAngle / 90f));
            targetSpeed = Mathf.Lerp(maxSpeed * 0.4f, maxSpeed, speedFactor);
            float dynamicTurnSpeed = Mathf.Lerp(turnSpeed * 0.3f, turnSpeed, speedFactor);

            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, dynamicTurnSpeed * Time.fixedDeltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);

            rb.MovePosition(transform.position + newDir * currentSpeed * Time.fixedDeltaTime);
        }
    }

    // void MoveAlongWaypoints()
    // {
    //     Transform[] currentPath = isOvertaking ? overtakeWaypoints : waypoints;
    //     int currentIndex = isOvertaking ? currentOvertakeIndex : currentWaypointIndex;

    //     if (currentIndex >= currentPath.Length) return;
    //         // Ostatni waypoint osiągnięty - zatrzymaj pojazd
    //         currentSpeed = 0f;
    //         targetSpeed = 0f;
    //         return;
    //     }

    //     Vector3 targetPos = currentPath[currentIndex].position;
    //     Vector3 flatTargetDir = targetPos - transform.position;
    //     flatTargetDir.y = 0;

    //     float angleToTarget = Vector3.Angle(transform.forward, flatTargetDir.normalized);


    //     float speedFactor = Mathf.Clamp01(1f - (angleToTarget / 90f)); 
    //     targetSpeed = Mathf.Lerp(maxSpeed * 0.4f, maxSpeed, speedFactor);


    //     float dynamicTurnSpeed = Mathf.Lerp(turnSpeed * 0.3f, turnSpeed, speedFactor);

    //     Quaternion targetRotation = Quaternion.LookRotation(flatTargetDir);
    //     transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, dynamicTurnSpeed * Time.deltaTime);

    //     rb.MovePosition(transform.position + transform.forward * currentSpeed * Time.fixedDeltaTime);

    //     float distance = flatTargetDir.magnitude;
    //     float dot = Vector3.Dot(transform.forward, flatTargetDir.normalized);

    //     if (distance < 3f || dot < 0f)
    //     {
    //         if (isOvertaking)
    //             currentOvertakeIndex++;
    //         else
    //             currentWaypointIndex++;


    //         targetSpeed = maxSpeed;
    //     }
    // }



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
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("AI"))
        {
            Debug.Log(gameObject.name + " zderzy� si� z " + collision.gameObject.name + "!");

            
            targetSpeed = maxSpeed * slowdownMultiplier;
            slowdownTimer = collisionSlowdownTime;

            
            Vector3 pushDir = (transform.position - collision.transform.position).normalized;
            rb.AddForce(pushDir * 300f);

            
            if (collisionSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(collisionSound);
            }
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
