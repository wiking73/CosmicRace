using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class cameraController : MonoBehaviour
{
    private GameObject attachedVehicle;

    public CinemachineVirtualCamera[] virtualCameras; 

    public int locationIndicator = 0;

    [SerializeField] private int activeCameraPriority = 20;
    [SerializeField] private int inactiveCameraPriority = 10;


    // Publiczna metoda do przypisania pojazdu z GameManagera
    public void AssignCameraToVehicle(GameObject vehicle)
    {
        attachedVehicle = vehicle;

        Transform virtualCamerasContainer = attachedVehicle.transform.Find("VirtualCameras");


        if (virtualCamerasContainer == null)
        {
            Debug.LogError("cameraController: Object 'VirtualCameras' not found as a child of vehicle '" + attachedVehicle.name + "'!");
            return;
        }

        virtualCameras = virtualCamerasContainer.GetComponentsInChildren<CinemachineVirtualCamera>();

        if (virtualCameras.Length == 0)
        {
            Debug.LogError("cameraController: Brak Cinemachine Virtual Cameras znalezionych w obiekcie '" + virtualCamerasContainer.name + "'. Upewnij się, że są tam umieszczone.");
            return;
        }

        Debug.Log("cameraController: Found " + virtualCameras.Length + " Virtual Cameras for vehicle '" + attachedVehicle.name + "'.");
        foreach(var cam in virtualCameras)
        {
            Debug.Log("- " + cam.name);
        }

        locationIndicator = Mathf.Clamp(locationIndicator, 0, virtualCameras.Length - 1);
        SwitchCamera(locationIndicator);
    }

    private void SwitchCamera(int newIndex)
    {
        if (newIndex < 0 || newIndex >= virtualCameras.Length)
        {
            Debug.LogWarning("cameraController: Index of camera out of scope: " + newIndex);
            return;
        }

        foreach (var cam in virtualCameras)
        {
            if (cam != null)
            {
                cam.Priority = inactiveCameraPriority;
            }
        }

        if (virtualCameras[newIndex] != null)
        {
            virtualCameras[newIndex].Priority = activeCameraPriority;
            Debug.Log("Switched to camera: " + virtualCameras[newIndex].name + " at index: " + newIndex);
        }
        else
        {
            Debug.LogError("cameraController: Camera with index " + newIndex + " is NULL!");
        }
    }

    private void Update()
    {
        if (virtualCameras == null || virtualCameras.Length == 0) return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            locationIndicator = (locationIndicator + 1) % virtualCameras.Length; 
            SwitchCamera(locationIndicator);
        }
    }
}