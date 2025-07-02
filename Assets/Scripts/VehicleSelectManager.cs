using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class VehicleSelectManager : MonoBehaviour
{
    public TextMeshProUGUI vehicleNameText;
    public TextMeshProUGUI massText;
    public TextMeshProUGUI accelerationText;
    public TextMeshProUGUI topSpeedText;
    public GameObject toRotate;
    public float rotateSpeed;

    public vehicleList listOfVehicles;
    public int vehiclePointer = 0;

    public static GameObject SelectedCarPrefab;

    private GameObject currentDisplayedVehicle;

    private void Awake()
    {
        vehiclePointer = PlayerPrefs.GetInt("VehiclePointer", 0);

        if (listOfVehicles == null || listOfVehicles.vehicles == null || listOfVehicles.vehicles.Count == 0)
        {
            Debug.LogError("VehicleSelectManager: 'listOfVehicles' is not assigned or is empty! Please assign a VehicleList ScriptableObject and add vehicles to it.", this);
            this.enabled = false; 
            return;
        }
        vehiclePointer = Mathf.Clamp(vehiclePointer, 0, listOfVehicles.vehicles.Count - 1);

        DisplayCurrentVehicle();
    }

    private void FixedUpdate()
    {
        if (toRotate != null)
        {
            toRotate.transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        }
    }

    public void rightButton()
    {
        if (listOfVehicles == null || listOfVehicles.vehicles.Count == 0) return;

        vehiclePointer++;
        if (vehiclePointer >= listOfVehicles.vehicles.Count)
        {
            vehiclePointer = 0; 
        }

        PlayerPrefs.SetInt("VehiclePointer", vehiclePointer);
        DisplayCurrentVehicle();
    }

    public void leftButton()
    {
        if (listOfVehicles == null || listOfVehicles.vehicles.Count == 0) return;

        vehiclePointer--;
        if (vehiclePointer < 0)
        {
            vehiclePointer = listOfVehicles.vehicles.Count - 1; 
        }

        PlayerPrefs.SetInt("VehiclePointer", vehiclePointer);
        DisplayCurrentVehicle();
    }

    private void DisplayCurrentVehicle()
    {
        if (currentDisplayedVehicle != null)
        {
            Destroy(currentDisplayedVehicle);
        }

        if (toRotate == null)
        {
            Debug.LogError("VehicleSelectManager: 'toRotate' GameObject is not assigned! Cannot display vehicle.", this);
            return;
        }

        SelectedCarPrefab = listOfVehicles.vehicles[vehiclePointer];

        currentDisplayedVehicle = Instantiate(listOfVehicles.vehicles[vehiclePointer], toRotate.transform);
        
        currentDisplayedVehicle.transform.localPosition = Vector3.zero;
        
        currentDisplayedVehicle.transform.localRotation = Quaternion.Euler(0f, 220f, 0f); 

        if (!currentDisplayedVehicle.CompareTag("Player"))
        {
            currentDisplayedVehicle.tag = "Player";
        }

        Rigidbody rb = currentDisplayedVehicle.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }
        WheelCollider[] wheelColliders = currentDisplayedVehicle.GetComponentsInChildren<WheelCollider>();
        foreach (WheelCollider wc in wheelColliders)
        {
            wc.enabled = false;
        }

        VehicleStats stats = currentDisplayedVehicle.GetComponent<VehicleStats>();

        if (stats != null)
        {
            vehicleNameText.text = "Name: " + stats.vehicleName;
            massText.text = "Mass: " + stats.mass + " kg";
            accelerationText.text = "Acceleration: " + stats.acceleration + " u/s";
            topSpeedText.text = "Max speed: " + stats.topSpeed + " km/h";
        }
        else
        {
            vehicleNameText.text = "No data";
            massText.text = "";
            accelerationText.text = "";
            topSpeedText.text = "";
        }
    }


    public void startGameButton()
    {
        vehiclePointer = PlayerPrefs.GetInt("VehiclePointer", 0); 
        SelectedCarPrefab = listOfVehicles.vehicles[vehiclePointer]; 

        string trackSceneToLoad = MenuManager.SelectedTrackSceneName;

        if (!string.IsNullOrEmpty(trackSceneToLoad))
        {
            SceneManager.LoadScene(trackSceneToLoad);
        }
        else
        {
            Debug.LogError("VehicleSelectManager: No track scene was selected in MenuManager! Loading SampleScene as fallback.");
            SceneManager.LoadScene("SampleScene");
        }
    }

    public void backToMainMenuButton()
    {
        SceneManager.LoadScene("mainMenu");
    }
}