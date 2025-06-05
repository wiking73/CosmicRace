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
        needleTransform.rotation = Quaternion.Euler(0, 0, minAngle);
    }
    void Update()
    {
        float speed = carRigidbody.GetComponent<CarController>().GetCurrentSpeed();
        float angle = Mathf.Lerp(minAngle, maxAngle, speed / maxSpeed);
        needleTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
