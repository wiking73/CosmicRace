using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class cameraController : MonoBehaviour
{

    internal enum updateMethod{
        fixedUpdate,
        update,
        lateUptade
    }
    [SerializeField]private updateMethod updateDemo;

    private GameObject attachedVehicle;
    private CinemachineVirtualCamera virtualCam;

    private GameObject cameraPositionFolder;
    private Transform[] camLocations;

    public int locationIndicator = 2;

    private void Start()
    {
        attachedVehicle = GameObject.FindGameObjectWithTag("Player");
        virtualCam = attachedVehicle.GetComponentInChildren<CinemachineVirtualCamera>();

        cameraPositionFolder = attachedVehicle.transform.Find("CAMERA").gameObject;
        camLocations = cameraPositionFolder.GetComponentsInChildren<Transform>();

        if (virtualCam != null)
        {
            virtualCam.Priority = 20;
            virtualCam.Follow = camLocations[locationIndicator];
            virtualCam.LookAt = camLocations[1];
        }
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (locationIndicator >= 4 || locationIndicator < 2)
                locationIndicator = 2;
            else
                locationIndicator++;

            virtualCam.Follow = camLocations[locationIndicator];
        }
    }
}
