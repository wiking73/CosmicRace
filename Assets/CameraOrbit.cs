using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target;
    public float distance = 12f;
    public float height = 1.5f;
    public float lookAtHeight = 1f;
    public float sensitivity = 2f;

    private float yaw = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yaw = target.eulerAngles.y;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        yaw += mouseX;
    }

    void LateUpdate()
    {
        Quaternion rotation = Quaternion.Euler(0f, yaw, 0f);

        Vector3 direction = rotation * Vector3.back * distance;
        Vector3 cameraPosition = target.position + direction + Vector3.up * height;
        transform.position = cameraPosition;


        Vector3 forwardOffset = target.forward * 5f;
        Vector3 lookAtPosition = target.position + forwardOffset + Vector3.up * (height * 0.5f);

        transform.LookAt(lookAtPosition);
    }

}

