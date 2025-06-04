using UnityEngine;



public class FollowSmooth : MonoBehaviour
{
    public Transform target; 
    public float smoothSpeed = 5f;
    public Vector3 offset;

    void LateUpdate()
    {
        if (!target) return;
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}


