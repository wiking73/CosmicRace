using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class CarController : MonoBehaviour
{
    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentbreakForce;
    private bool isBreaking;
    private float originalMotorForce;
    public TextMeshProUGUI FlipCar;
    public TextMeshProUGUI speedText;


    [SerializeField] private float resetHeight = 1.0f;
    [SerializeField] private KeyCode resetKey = KeyCode.R;
    // Settings
    [SerializeField] private float motorForce, breakForce, maxSteerAngle;
    [SerializeField] private float boostMultiplier = 3000f;

    // Wheel Colliders
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    // Wheels
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

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
    }
    private bool IsCarFlipped()
    {
        return Vector3.Dot(transform.up, Vector3.down) > 0.5f;
    }
    private void Update()
    {
        if (Input.GetKeyDown(resetKey))
        {
            ResetCarOrientation();
        }
        if (FlipCar != null)
        {
            FlipCar.gameObject.SetActive(IsCarFlipped());
        }
        UpdateSpeedDisplay();
    }
    private void UpdateSpeedDisplay()
    {
        if (speedText != null)
        {
            float speed = GetComponent<Rigidbody>().linearVelocity.magnitude * 3.6f; 
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
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce;
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

    private void Start()
    {
        originalMotorForce = motorForce; 

        if (SceneManager.GetActiveScene().name == "VehicleSelectScene")
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }
        }
    }
    public IEnumerator ActivateBoost(float multiplier, float duration) 
    {
        float originalMotorForce = motorForce;
        motorForce += multiplier; 
        yield return new WaitForSeconds(duration);
        motorForce = originalMotorForce;
    }
    private void ResetCarOrientation()
    {
        
        transform.position += Vector3.up * resetHeight;    
        Vector3 uprightRotation = new Vector3(0, transform.eulerAngles.y, 0);
        transform.eulerAngles = uprightRotation;

        
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
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
        return GetComponent<Rigidbody>().linearVelocity.magnitude * 3.6f; 
    }




}