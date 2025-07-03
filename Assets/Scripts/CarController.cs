using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;


public class CarController : MonoBehaviour
{

    [Header("General")]
    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentbreakForce;
    private bool isBreaking;
    private float originalMotorForce;
    public TextMeshProUGUI speedText;

    [Header("Engine Sound")]
    public AudioClip engineSoundClip;
    public float minPitch = 0.8f;
    public float maxPitch = 1.5f;
    public float minVolume = 0.2f;
    public float maxVolume = 1.0f;


    //HEALTH

    public HealthUI healthUI;
    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;



    private AudioSource engineAudioSource; 
    private Rigidbody carRigidbody;

    [SerializeField] private LayerMask trackLayer;
    [SerializeField] private float trackCheckDistance = 0.1f;
    private Vector3 lastValidPosition;

    private bool isOffTrack = false;
    private float offTrackTimer = 0f;
    [SerializeField] private float respawnDelay = 1.5f;

    [Header("Respawn & Reset Controls")]
    [SerializeField] private float resetHeight = 1.0f;
    [SerializeField] private KeyCode resetOrientationKey = KeyCode.R;
    [SerializeField] private KeyCode manualRespawnKey = KeyCode.P;

    [Header("Settings")]
    [SerializeField] private float motorForce, breakForce, maxSteerAngle;
    [SerializeField] private float boostMultiplier = 3000f;

    // Wheel Colliders
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    // Wheels
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    private CameraOrbit cameraOrbit;

    private VehicleStats vehicleStats;

   


    private void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().name == "VehicleSelectScene")
            return;

        if (!GameManager.Instance.raceStarted)
            return;
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        UpdateEngineSound();

        vehicleStats = GetComponent<VehicleStats>();

        if (verticalInput > 0)
        {
            ApplyAccelerationForce(vehicleStats.acceleration); // albo vehicleStats.acceleration
        }

    }

    void ApplyAccelerationForce(float desiredAcceleration)
    {
        float force = carRigidbody.mass * desiredAcceleration;
        carRigidbody.AddForce(transform.forward * force);
    }

    private bool IsCarFlipped()
    {
        float angle = Vector3.Angle(transform.up, Vector3.up);
        float flipThresholdAngle = 60f;

        return angle > flipThresholdAngle;
    }
    private void Update()
    {
        if (Input.GetKeyDown(resetOrientationKey))
        {
            ResetCarOrientation();
        }

        if (Input.GetKeyDown(manualRespawnKey))
        {
            RespawnToLastValid();
        }
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowFlipCarPrompt(IsCarFlipped());
        }

        UpdateSpeedDisplay();
        if (SceneManager.GetActiveScene().name != "SampleScene")
        {            
            return;
        }
        CheckTrackUnderneath();

    }
    private void UpdateSpeedDisplay()
    {
        if (speedText != null && carRigidbody != null)
        {
            float speed = GetCurrentSpeed();
            speedText.text = Mathf.RoundToInt(speed).ToString() + " km/h";
        }
    }



    private void GetInput()
    {
        // Steering Input
        horizontalInput = Input.GetAxis("Horizontal");

        // Acceleration Input
        verticalInput = Input.GetAxis("Vertical");

        // Breaking Input
        isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        float currentSpeed = carRigidbody.linearVelocity.magnitude * 3.6f;

        Debug.LogWarning("Current speed" +  currentSpeed);

        if (vehicleStats != null && currentSpeed < vehicleStats.topSpeed)
        {
            frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
            frontRightWheelCollider.motorTorque = verticalInput * motorForce;
        }
        else
        {
            frontLeftWheelCollider.motorTorque = 0;
            frontRightWheelCollider.motorTorque = 0;
        }

        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private void Awake()
    {
        carRigidbody = GetComponent<Rigidbody>();

        engineAudioSource = gameObject.AddComponent<AudioSource>();
        engineAudioSource.clip = engineSoundClip;
        engineAudioSource.loop = true;
        engineAudioSource.playOnAwake = false;
        engineAudioSource.spatialBlend = 1.0f;
        engineAudioSource.volume = minVolume;
        engineAudioSource.pitch = minPitch;


        engineAudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        engineAudioSource.minDistance = 5f;
        engineAudioSource.maxDistance = 200f;

        originalMotorForce = motorForce;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "VehicleSelectScene")
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }
        }

        currentHealth = maxHealth;
        UpdateHealthUI();
        if (healthUI == null)
            healthUI = FindObjectOfType<HealthUI>();

        cameraOrbit = FindObjectOfType<CameraOrbit>();

        if (engineSoundClip != null)
        {
            engineAudioSource.Play();
        }

        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.RegisterEngineAudioSource(engineAudioSource);
        }
        else
        {
            Debug.LogError("CarController: SFXManager.Instance is null. Cannot register engine audio source.", this);
        }

        carRigidbody = GetComponent<Rigidbody>();
        engineAudioSource = GetComponent<AudioSource>();

        vehicleStats = GetComponent<VehicleStats>();
        if (vehicleStats != null)
        {
            motorForce = vehicleStats.acceleration * 300f; // np. skalowanie przyspieszenia na motorForce
            Debug.LogWarning("Motot force: " + motorForce);
        }
        else
        {
            Debug.LogError("Brak VehicleStats na pojeździe: " + gameObject.name);
        }

    }

    private void OnDestroy()
    {
        if (SFXManager.Instance != null && engineAudioSource != null)
        {
            SFXManager.Instance.UnregisterEngineAudioSource(engineAudioSource);
        }
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowFlipCarPrompt(false);
        }
    }

    public IEnumerator ActivateBoost(float multiplier, float duration)
    {
        motorForce += multiplier;
        yield return new WaitForSeconds(duration);
        motorForce = this.originalMotorForce;
    }

    private void ResetCarOrientation()
    {
        transform.position += Vector3.up * resetHeight;
        Vector3 uprightRotation = new Vector3(0, transform.eulerAngles.y, 0);
        transform.eulerAngles = uprightRotation;

        if (cameraOrbit != null)
        {
            cameraOrbit.ForceRearView();
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowFlipCarPrompt(false);
        }
    }

    public IEnumerator BoostMassAndSpeed(float massReduction, float motorBoost, float duration)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null) yield break;

        float originalMass = rb.mass;
        float originalMotor = motorForce;

        
        rb.mass = Mathf.Max(1f, originalMass - massReduction);
        motorForce += motorBoost;

      
        yield return new WaitForSeconds(duration);

        
        rb.mass = originalMass;
        motorForce = originalMotor;
    }
    public float GetCurrentSpeed()
    {
        if (carRigidbody != null)
        {
            return carRigidbody.linearVelocity.magnitude * 3.6f; 
        }
        return 0f;
    }

    void UpdateEngineSound()
    {
        if (engineAudioSource == null || carRigidbody == null || SFXManager.Instance == null) return;

        float currentSpeed = carRigidbody.linearVelocity.magnitude;

        float speedNormalized = Mathf.InverseLerp(0f, 30f, currentSpeed);

        engineAudioSource.pitch = Mathf.Lerp(minPitch, maxPitch, speedNormalized);
        engineAudioSource.volume = Mathf.Lerp(minVolume, maxVolume, speedNormalized);
    
        engineAudioSource.mute = SFXManager.Instance.IsSFXMuted();

        if (GameManager.Instance != null)
        {
            if (!GameManager.Instance.raceStarted && engineAudioSource.isPlaying)
            {
                engineAudioSource.mute = true;
            }
            else if (GameManager.Instance.raceStarted && !engineAudioSource.isPlaying && !SFXManager.Instance.IsSFXMuted())
            {
                engineAudioSource.mute = false;
            }
        }
        else
        {
            if (!SFXManager.Instance.IsSFXMuted())
            {
                engineAudioSource.mute = false;
            }
        }
    }

	private void RespawnToLastValid()
    {
        Transform bestPoint = null;

        if (TrackManager.Instance != null)
        {
            bestPoint = TrackManager.Instance.GetNearestRespawnPoint(transform.position);
        }
        else
        {
            Debug.LogError("CarController: Brak instancji TrackManager w scenie! Nie można zrespawnować samochodu. Upewnij się, że GameObject 'TrackManager' z komponentem 'TrackManager.cs' istnieje i jest aktywny.");
            return;
        }

        if (bestPoint == null)
        {
            Debug.LogWarning("Brak dostępnego punktu respawnu z TrackManager. Sprawdź, czy któryś TrackChecker został aktywowany i ma przypisany TrackWaypointContainer z punktami, lub ustaw domyślny Fallback Respawn Point w TrackManager.", this);
            return;
        }

        transform.position = bestPoint.position + Vector3.up * 1f;
        transform.rotation = Quaternion.Euler(0, bestPoint.eulerAngles.y, 0);

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        isOffTrack = false;
        offTrackTimer = 0f;

        Debug.Log("Respawn na punkt: " + bestPoint.name);
    }

    private void CheckTrackUnderneath()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);
        bool onTrack = Physics.Raycast(ray, trackCheckDistance, trackLayer);

        if (onTrack)
        {
            lastValidPosition = transform.position;
            isOffTrack = false;
            offTrackTimer = 0f;
        }
        else
        {
            if (!isOffTrack)
            {
                isOffTrack = true;
                offTrackTimer = 0f;
            }
            else
            {
                offTrackTimer += Time.deltaTime;
                if (offTrackTimer >= respawnDelay)
                {
                    RespawnToLastValid();
                    isOffTrack = false;
                }
            }
        }
    }


    public IEnumerator FreezeAndTeleport(float duration)
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true; 
        }

        yield return new WaitForSeconds(duration * 0.5f); 

        RespawnToLastValid(); 

        yield return new WaitForSeconds(duration * 0.5f); 

        if (rb != null)
        {
            rb.isKinematic = false; 
        }
    }

    public IEnumerator FreezeCar(float duration)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            float originalMotor = motorForce;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            motorForce = 0f;


            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowTemporaryMessage("Zatrzymano przez przeszkode!", true);
            }


            yield return new WaitForSeconds(duration);

            rb.isKinematic = false;
            motorForce = originalMotor;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowTemporaryMessage("", false);
            }
        }
    }
    private void UpdateHealthUI()
    {
        if (healthUI != null)
        {
            healthUI.UpdateHP(currentHealth, maxHealth);
        }
    }
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
    }


    private void Die()
    {

        RaceManager.Instance.GameOver();


        if (carRigidbody != null)
        {
            carRigidbody.linearVelocity = Vector3.zero;
            carRigidbody.angularVelocity = Vector3.zero;
            carRigidbody.isKinematic = true;
        }

        
        this.enabled = false;

        
        if (engineAudioSource != null)
        {
            engineAudioSource.Stop();
        }

        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowTemporaryMessage("Game Over", true);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("AI"))
        {
            
            TakeDamage(20);
        }
    }
}