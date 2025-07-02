using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class cameraController : MonoBehaviour
{
    private GameObject attachedVehicle;

    public CinemachineVirtualCamera[] virtualCameras; 
    private CinemachineVirtualCamera orbitVirtualCamera;
    private CameraOrbit orbitScript;

    public int locationIndicator = 0;
    private bool isOrbitActive = false;

    [SerializeField] private int activeCameraPriority = 20;
    [SerializeField] private int inactiveCameraPriority = 10;
    [SerializeField] private int orbitCameraPriority = 25;


    public void AssignCameraToVehicle(GameObject vehicle)
    {
        attachedVehicle = vehicle;

        Transform virtualCamerasContainer = attachedVehicle.transform.Find("VirtualCameras");
        Transform followPointTransform = attachedVehicle.transform.Find("FollowPoint");


        if (virtualCamerasContainer == null)
        {
            Debug.LogError("cameraController: Object 'VirtualCameras' not found as a child of vehicle '" + attachedVehicle.name + "'!");
            return;
        }

        if (followPointTransform == null)
        {
            Debug.LogError("cameraController: Object 'FollowPoint' not found as a child of vehicle '" + attachedVehicle.name + "'!");
            return;
        }

        virtualCameras = virtualCamerasContainer.GetComponentsInChildren<CinemachineVirtualCamera>();

        if (virtualCameras.Length == 0)
        {
            Debug.LogError("cameraController: No Cinemachine Virtual Cameras found in object '" + virtualCamerasContainer.name + "'.");
            return;
        }

        Debug.Log("cameraController: Found " + virtualCameras.Length + " Virtual Cameras for vehicle '" + attachedVehicle.name + "'.");
        foreach(var cam in virtualCameras)
        {
            Debug.Log("- " + cam.name);
        }

        orbitVirtualCamera = followPointTransform.GetComponentInChildren<CinemachineVirtualCamera>();
        if (orbitVirtualCamera != null)
        {
            orbitScript = orbitVirtualCamera.GetComponent<CameraOrbit>();
            if (orbitScript == null)
            {
                Debug.LogWarning("cameraController: CameraOrbit script not found on the Virtual Camera under FollowPoint.");
            }
            Debug.Log("cameraController: Found Orbit Virtual Camera: " + orbitVirtualCamera.name);
        }
        else
        {
            Debug.LogError("cameraController: Cinemachine Virtual Camera not found under 'FollowPoint'. Cannot use CameraOrbit functionality.");
        }

        locationIndicator = 0;
        isOrbitActive = false;
        SwitchCameraMode(false);
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

        if (orbitVirtualCamera != null)
        {
            orbitVirtualCamera.Priority = inactiveCameraPriority;
            if (orbitScript != null) orbitScript.enabled = false;
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
    
    private void SwitchOrbitCamera(bool instantSwitch = false)
    {
        foreach (var cam in virtualCameras)
        {
            if (cam != null)
            {
                cam.Priority = inactiveCameraPriority;
            }
        }

        if (orbitVirtualCamera != null)
        {
            orbitVirtualCamera.Priority = orbitCameraPriority;
            if (orbitScript != null) 
            {
                orbitScript.enabled = true;
                if (instantSwitch) orbitScript.GoToNextViewModeInstant();
                else orbitScript.GoToNextViewMode();
            }
            Debug.Log("Switched to Orbit Camera.");
        }
        else
        {
            Debug.LogError("cameraController: Orbit Virtual Camera is NULL! Cannot switch to Orbit mode.");
        }
    }

    private void SwitchCameraMode(bool activateOrbit)
    {
        if (activateOrbit)
        {
            if (orbitVirtualCamera == null)
            {
                Debug.LogWarning("Cannot switch to Orbit mode, Orbit Virtual Camera not found.");
                return;
            }
            isOrbitActive = true;
            SwitchOrbitCamera(true);
        }
        else
        {
            isOrbitActive = false;
            SwitchCamera(locationIndicator);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isOrbitActive)
            {
                SwitchCameraMode(false); 
                locationIndicator = 0;
                SwitchCamera(locationIndicator);
            }
            else
            {
                locationIndicator = (locationIndicator + 1) % virtualCameras.Length; 
                SwitchCamera(locationIndicator);
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (!isOrbitActive)
            {
                SwitchCameraMode(true); 
            }
            else
            {
                if (orbitScript != null)
                {
                    orbitScript.GoToNextViewModeInstant();
                    Debug.Log("CameraOrbit: Switched view mode.");
                }
            }
        }
    }
}