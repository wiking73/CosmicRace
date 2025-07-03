using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target;
    public float followDistance = 10f;
    public float followHeight = 2f;
    public float flippedYOffset = 6f;
    public float positionDamping = 0.1f;
    public float rotationDamping = 0.1f;
    public float maxRotationLag = 30f;

    private enum ViewMode { Rear, Front, Side }
    private ViewMode currentView = ViewMode.Rear;
    private Vector3 targetOffset;
    private Vector3 positionVelocity;
    private float rotationVelocityY;

    private Vector3 savedLocalOffset;
    private Quaternion savedLocalRotation;
    private bool wasFlippedLastFrame = false;

    private bool forceInstantRearView = false;

    void Start()
    {
        SetViewOffset();
        this.enabled = false;
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.K))
        // {
        //     currentView = (ViewMode)(((int)currentView + 1) % System.Enum.GetValues(typeof(ViewMode)).Length);
        //     SetViewOffset();

        //     if (target != null)
        //     {
        //         transform.position = target.position + target.rotation * targetOffset;
        //         Vector3 lookDirection = (target.position + Vector3.up * followHeight) - transform.position;
        //         transform.rotation = Quaternion.LookRotation(lookDirection);

        //         positionVelocity = Vector3.zero;
        //         rotationVelocityY = 0f;
        //     }
        // }
    }

    void LateUpdate()
    {
        if (target == null) return;

        bool isFlipped = Vector3.Dot(target.up, Vector3.down) > 0.5f;

        if (forceInstantRearView && !isFlipped)
        {
            forceInstantRearView = false;

            transform.position = target.position + target.rotation * targetOffset;
            Vector3 lookDirection = (target.position + Vector3.up * followHeight) - transform.position;
            transform.rotation = Quaternion.LookRotation(lookDirection);

            positionVelocity = Vector3.zero;
            rotationVelocityY = 0f;
            wasFlippedLastFrame = false;
            return;
        }

        Vector3 desiredPosition;
        Quaternion desiredRotation;

        if (isFlipped)
        {
            if (!wasFlippedLastFrame)
            {
                savedLocalOffset = target.InverseTransformPoint(transform.position);
                savedLocalRotation = Quaternion.Inverse(target.rotation) * transform.rotation;
            }

            desiredPosition = target.position + Vector3.up * (followHeight + flippedYOffset);
            desiredRotation = Quaternion.Euler(90f, 0f, 0f);
        }
        else
        {
            if (wasFlippedLastFrame)
            {
                desiredPosition = target.TransformPoint(savedLocalOffset);
                desiredRotation = target.rotation * savedLocalRotation;
            }
            else
            {
                desiredPosition = target.position + target.rotation * targetOffset;

                Vector3 lookDirection = (target.position + Vector3.up * followHeight) - desiredPosition;
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

                float targetYAngle = targetRotation.eulerAngles.y;
                float currentYAngle = transform.eulerAngles.y;

                float angleDiff = Mathf.DeltaAngle(currentYAngle, targetYAngle);
                if (Mathf.Abs(angleDiff) > maxRotationLag)
                    targetYAngle = currentYAngle + Mathf.Sign(angleDiff) * maxRotationLag;

                float smoothedYAngle = Mathf.SmoothDampAngle(
                    currentYAngle,
                    targetYAngle,
                    ref rotationVelocityY,
                    rotationDamping
                );

                desiredRotation = Quaternion.Euler(
                    targetRotation.eulerAngles.x,
                    smoothedYAngle,
                    0
                );
            }
        }

        wasFlippedLastFrame = isFlipped;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref positionVelocity,
            positionDamping
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRotation,
            1 - Mathf.Exp(-rotationDamping * Time.deltaTime)
        );
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

    public void ResetFlippedState()
    {
        wasFlippedLastFrame = false;
    }

    public void ForceRearView()
    {
        currentView = ViewMode.Rear;
        SetViewOffset();
        forceInstantRearView = true;
    }

     public void GoToNextViewMode()
    {
        currentView = (ViewMode)(((int)currentView + 1) % System.Enum.GetValues(typeof(ViewMode)).Length);
        SetViewOffset();
    }

    public void GoToNextViewModeInstant()
    {
        GoToNextViewMode();
        
        if (target != null)
        {
            transform.position = target.position + target.rotation * targetOffset;
            Vector3 lookDirection = (target.position + Vector3.up * followHeight) - transform.position;
            transform.rotation = Quaternion.LookRotation(lookDirection);

            positionVelocity = Vector3.zero;
            rotationVelocityY = 0f;
        }
    }
}
