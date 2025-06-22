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

        Debug.Log(name + " zakoñczy³ boost.");
    }

    void MoveAlongWaypoints()
    {
        Transform[] currentPath = isOvertaking ? overtakeWaypoints : waypoints;
        int currentIndex = isOvertaking ? currentOvertakeIndex : currentWaypointIndex;

        if (currentIndex >= currentPath.Length) return;

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
            if (isOvertaking)
                currentOvertakeIndex++;
            else
                currentWaypointIndex++;

           
            targetSpeed = maxSpeed;
        }
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
    public void SetWaypoints(Transform[] newWaypoints, Transform[] newOvertakeWaypoints = null)
{
    waypoints = newWaypoints;
    overtakeWaypoints = newOvertakeWaypoints ?? new Transform[0];
    currentWaypointIndex = 0;
    currentOvertakeIndex = 0;
    isOvertaking = false;

    Debug.Log(name + " zmieni³ trasê na nowe waypointy.");
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
