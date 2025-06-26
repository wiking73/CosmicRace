using UnityEngine;

public class Danger : MonoBehaviour
{
    public AudioClip dangerSound;
    public GameObject effectOnHit;

    public float flipForce = 500f;
    public float torqueForce = 300f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (dangerSound && SFXManager.Instance != null)
                SFXManager.Instance.PlaySFX3D(dangerSound, transform.position);

            if (effectOnHit)
                Instantiate(effectOnHit, transform.position, Quaternion.identity);

            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Debug.Log("Car flipped!");

               
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

               
                other.transform.Rotate(180f, 0f, 0f, Space.Self);

                
                rb.AddForce(Vector3.up * flipForce, ForceMode.Impulse);
                rb.AddTorque(Vector3.right * torqueForce, ForceMode.Impulse);
            }

            Destroy(gameObject);
        }
    }
}
