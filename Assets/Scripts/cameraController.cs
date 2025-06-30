using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class cameraController : MonoBehaviour
{
    private GameObject attachedVehicle;
    private CinemachineVirtualCamera virtualCam;

    private GameObject cameraPositionFolder;
    private Transform[] camLocations;

    public int locationIndicator = 2;

    // Publiczna metoda do przypisania pojazdu z GameManagera
    public void AssignCameraToVehicle(GameObject vehicle)
    {
        attachedVehicle = vehicle;
        virtualCam = attachedVehicle.GetComponentInChildren<CinemachineVirtualCamera>();

        if (attachedVehicle.transform.Find("CAMERA") == null)
        {
            Debug.LogError("CAMERA folder nie znaleziony w pojeÅºdzie!");
            return;
        }

        cameraPositionFolder = attachedVehicle.transform.Find("CAMERA").gameObject;
        camLocations = cameraPositionFolder.GetComponentsInChildren<Transform>();

        if (virtualCam != null)
        {
            Debug.Log("Virtual Camera found: " + virtualCam.name);
            virtualCam.Priority = 20;
            virtualCam.Follow = camLocations[locationIndicator];
            virtualCam.LookAt = camLocations[1];

            Debug.Log("Camera locations initialized. Current Follow: " + camLocations[locationIndicator].name);
        }
        else
        {
            Debug.LogError("No Virtual Camera found in the attached vehicle.");
        }
    }

    private void Update()
    {
        if (attachedVehicle == null || virtualCam == null || camLocations == null) return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (locationIndicator >= 4 || locationIndicator < 2)
            {
                locationIndicator = 2;
                Debug.Log("Location Indicator: " + locationIndicator + " camlocations: " + camLocations[locationIndicator].name);
            }
            else
            {
                locationIndicator++;
                Debug.Log("else Location Indicator: " + locationIndicator);
            }

            virtualCam.Follow = camLocations[locationIndicator];
        }
    }
}