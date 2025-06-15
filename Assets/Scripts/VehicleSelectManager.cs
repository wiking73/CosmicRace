using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSelectManager : MonoBehaviour
{
    public GameObject toRotate;
    public float rotateSpeed;
    public vehicleList listOfVehicles;
    public int vehiclePointer = 0;

    private void Awake()
    {
        PlayerPrefs.SetInt("VehiclePointer", 0);
        vehiclePointer = PlayerPrefs.GetInt("VehiclePointer");

        GameObject childObjeect = Instantiate(listOfVehicles.vehicles[vehiclePointer], Vector3.zero, Quaternion.identity) as GameObject;
        childObjeect.transform.parent = toRotate.transform;

        childObjeect.transform.localPosition = new Vector3(0f, 0f, 0f);
        childObjeect.transform.localRotation = Quaternion.identity;

        childObjeect.transform.localRotation = Quaternion.Euler(0f, 220f, 0f);

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
            GameObject childObjeect = Instantiate(listOfVehicles.vehicles[vehiclePointer], Vector3.zero, Quaternion.identity) as GameObject;
            childObjeect.transform.parent = toRotate.transform;

            childObjeect.transform.localPosition = new Vector3(0f, 0f, 0f);
            childObjeect.transform.localRotation = Quaternion.identity;

            childObjeect.transform.localRotation = Quaternion.Euler(0f, 220f, 0f);
        }
    }

    public void leftButton()
    {
        if (vehiclePointer > 0)
        {
            Destroy(GameObject.FindGameObjectWithTag("Player"));
            vehiclePointer--;
            PlayerPrefs.SetInt("VehiclePointer", vehiclePointer);
            GameObject childObjeect = Instantiate(listOfVehicles.vehicles[vehiclePointer], Vector3.zero, Quaternion.identity) as GameObject;
            childObjeect.transform.parent = toRotate.transform;

            childObjeect.transform.localPosition = new Vector3(0f, 0f, 0f);
            childObjeect.transform.localRotation = Quaternion.identity;

            childObjeect.transform.localRotation = Quaternion.Euler(0f, 220f, 0f);
        }
    }
    
}
