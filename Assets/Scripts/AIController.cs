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

    void MoveAlongWaypoints()
    {
        if (isOvertaking)
        {
            if (currentOvertakeIndex >= overtakeWaypoints.Length) return;

            Vector3 targetDir = overtakeWaypoints[currentOvertakeIndex].position - transform.position;
            targetDir.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(targetDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

            rb.MovePosition(transform.position + transform.forward * currentSpeed * Time.fixedDeltaTime);

            if (Vector3.Distance(transform.position, overtakeWaypoints[currentOvertakeIndex].position) < 3f)
            {
                currentOvertakeIndex++;
                if (currentOvertakeIndex >= overtakeWaypoints.Length)
                {
                    isOvertaking = false;
                }
            }
        }
        else
        {
            if (currentWaypointIndex >= waypoints.Length) return;

            Vector3 targetDir = waypoints[currentWaypointIndex].position - transform.position;
            targetDir.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(targetDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

            rb.MovePosition(transform.position + transform.forward * currentSpeed * Time.fixedDeltaTime);

            if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 3f)
            {
                currentWaypointIndex++;
            }
        }
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
