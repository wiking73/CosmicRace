using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VehicleSelectManager : MonoBehaviour
{
    public GameObject toRotate;
    public float rotateSpeed;
    public vehicleList listOfVehicles;
    public int vehiclePointer = 0;

    public static GameObject SelectedCarPrefab; 

    private void Awake()
    {
        vehiclePointer = PlayerPrefs.GetInt("VehiclePointer", 0);

        SelectedCarPrefab = listOfVehicles.vehicles[vehiclePointer]; 

        GameObject childObject = Instantiate(listOfVehicles.vehicles[vehiclePointer], Vector3.zero, Quaternion.identity) as GameObject;
        childObject.transform.parent = toRotate.transform;

        childObject.transform.localPosition = new Vector3(0f, 0f, 0f);
        childObject.transform.localRotation = Quaternion.identity;

        childObject.transform.localRotation = Quaternion.Euler(0f, 220f, 0f);

    }

    private void FixedUpdate()
    {
        toRotate.transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    public void rightButton()
    {
        if (vehiclePointer < listOfVehicles.vehicles.Count - 1)
        {
            Destroy(GameObject.FindGameObjectWithTag("Player"));
            vehiclePointer++;
            PlayerPrefs.SetInt("VehiclePointer", vehiclePointer);

            SelectedCarPrefab = listOfVehicles.vehicles[vehiclePointer]; 

            GameObject childObject = Instantiate(listOfVehicles.vehicles[vehiclePointer], Vector3.zero, Quaternion.identity) as GameObject;
            childObject.transform.parent = toRotate.transform;

            childObject.transform.localPosition = new Vector3(0f, 0f, 0f);
            childObject.transform.localRotation = Quaternion.identity;

            childObject.transform.localRotation = Quaternion.Euler(0f, 220f, 0f);
        }
    }

    public void leftButton()
    {
        if (vehiclePointer > 0)
        {
            Destroy(GameObject.FindGameObjectWithTag("Player"));
            vehiclePointer--;
            PlayerPrefs.SetInt("VehiclePointer", vehiclePointer);

            SelectedCarPrefab = listOfVehicles.vehicles[vehiclePointer]; 

            GameObject childObject = Instantiate(listOfVehicles.vehicles[vehiclePointer], Vector3.zero, Quaternion.identity) as GameObject;
            childObject.transform.parent = toRotate.transform;

            childObject.transform.localPosition = new Vector3(0f, 0f, 0f);
            childObject.transform.localRotation = Quaternion.identity;

            childObject.transform.localRotation = Quaternion.Euler(0f, 220f, 0f);
        }
    }

    public void startGameButton()
    {
        SelectedCarPrefab = listOfVehicles.vehicles[PlayerPrefs.GetInt("VehiclePointer", 0)];
        SceneManager.LoadScene("SampleScene");
    }
}
