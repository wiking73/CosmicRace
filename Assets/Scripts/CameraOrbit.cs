using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target;
    public float followDistance = 10f;
    public float followHeight = 2f;
    public float positionDamping = 5f;       // Op�nienie pozycji
    public float rotationDamping = 3f;        // Op�nienie rotacji
    public float maxRotationLag = 30f;        // Maksymalne op�nienie skr�tu (stopnie)

    private enum ViewMode { Rear, Front, Side }
    private ViewMode currentView = ViewMode.Rear;
    private Vector3 targetOffset;
    private Vector3 positionVelocity;
    private float rotationVelocityY;

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
        // Oblicz docelow� pozycj� z offsetem
        Vector3 desiredPosition = target.position + target.rotation * targetOffset;

        // P�ynne �ledzenie pozycji z op�nieniem
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref positionVelocity,
            positionDamping * Time.deltaTime
        );

        // Oblicz docelow� rotacj� z uwzgl�dnieniem pojazdu
        Quaternion targetRotation = Quaternion.LookRotation(
            (target.position + Vector3.up * followHeight) - transform.position
        );

        // Efekt op�nionego skr�tu
        float targetYAngle = targetRotation.eulerAngles.y;
        float currentYAngle = transform.eulerAngles.y;

        // Ogranicz maksymalne op�nienie skr�tu
        if (Mathf.Abs(Mathf.DeltaAngle(currentYAngle, targetYAngle)) > maxRotationLag)
        {
            targetYAngle = currentYAngle + Mathf.Sign(targetYAngle - currentYAngle) * maxRotationLag;
        }

        // P�ynna interpolacja rotacji
        float smoothedYAngle = Mathf.SmoothDampAngle(
            currentYAngle,
            targetYAngle,
            ref rotationVelocityY,
            rotationDamping * Time.deltaTime
        );

        // Zastosuj now� rotacj�
        transform.rotation = Quaternion.Euler(
            targetRotation.eulerAngles.x,
            smoothedYAngle,
            0
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
}
