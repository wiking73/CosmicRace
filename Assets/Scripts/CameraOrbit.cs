using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target;
    public float followDistance = 10f;
    public float followHeight = 2f;
    public float rotationDamping = 3f;
    public float positionDamping = 5f;

    private enum ViewMode { Rear, Front, Side, Top }
    private ViewMode currentView = ViewMode.Rear;

    private Vector3 targetOffset;

    void Start()
    {
        SetViewOffset(); 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            
            currentView = (ViewMode)(((int)currentView + 1) % System.Enum.GetValues(typeof(ViewMode)).Length);
            SetViewOffset();
        }
    }

    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + target.rotation * targetOffset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * positionDamping);
        transform.position = smoothedPosition;

        
        Quaternion desiredRotation = Quaternion.LookRotation(target.position + Vector3.up * followHeight - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationDamping);
    }

    void SetViewOffset()
    {
        switch (currentView)
        {
            case ViewMode.Rear:
                targetOffset = new Vector3(0, followHeight, -followDistance);
                break;
            case ViewMode.Front:
                targetOffset = new Vector3(0, followHeight, followDistance * 0.6f);
                break;
            case ViewMode.Side:
                targetOffset = new Vector3(followDistance * 0.7f, followHeight, 0);
                break;

        }
    }
}
