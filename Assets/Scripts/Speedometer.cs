using UnityEngine;

public class Speedometer : MonoBehaviour
{
    public Rigidbody carRigidbody;
    public RectTransform needleTransform;
    public float minAngle = 120f;
    public float maxAngle = -20f;
    public float maxSpeed = 200f;

    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.playerCarInstance != null)
        {
            carRigidbody = GameManager.Instance.playerCarInstance.GetComponent<Rigidbody>();
        }
        else
        {
            Debug.LogError("Speedometer: Could not find player car instance. Speedometer will not function.", this);
            enabled = false;
            return;
        }

        if (needleTransform != null)
        {
            needleTransform.rotation = Quaternion.Euler(0, 0, minAngle);
        }
        else
        {
            Debug.LogWarning("Speedometer: needleTransform is not assigned. Speedometer visual will not work.", this);
        }
    }
    void Update()
    {
        if (carRigidbody != null && needleTransform != null)
        {
            float speed = carRigidbody.GetComponent<CarController>().GetCurrentSpeed();
            float angle = Mathf.Lerp(minAngle, maxAngle, speed / maxSpeed);
            needleTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
