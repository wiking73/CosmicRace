using UnityEngine;

public class AIController : MonoBehaviour
{
    public Transform[] waypoints;
    public Transform[] overtakeWaypoints;  // trasa wyprzedzania
    public float speed = 10f;
    public float turnSpeed = 5f;
    public float rayDistance = 7f;
    public float collisionSlowdownTime = 2f;
    public float slowdownMultiplier = 0.5f;

    private int currentWaypointIndex = 0;
    private int currentOvertakeIndex = 0;
    private bool isOvertaking = false;
    private Rigidbody rb;
    private float originalSpeed;
    private float slowdownTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalSpeed = speed;
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
                speed = originalSpeed;
            }
        }

        MoveAlongWaypoints();
        DetectObstacles();
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

            rb.MovePosition(transform.position + transform.forward * speed * Time.fixedDeltaTime);

            if (Vector3.Distance(transform.position, overtakeWaypoints[currentOvertakeIndex].position) < 3f)
            {
                currentOvertakeIndex++;

                // jeœli skoñczyliœmy trasê wyprzedzania — wróæ na g³ówny tor
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

            rb.MovePosition(transform.position + transform.forward * speed * Time.fixedDeltaTime);

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
            speed *= slowdownMultiplier;
            slowdownTimer = collisionSlowdownTime;

            // Delikatny odskok
            Vector3 pushDir = (transform.position - collision.transform.position).normalized;
            rb.AddForce(pushDir * 300f);
        }
    }

}
