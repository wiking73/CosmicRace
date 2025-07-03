using UnityEngine;

public class Danger : MonoBehaviour
{
    public AudioClip dangerSound;
    public GameObject effectOnHit;

    public float flipForce = 5f;
    public float torqueForce = 3f;
    public float liftAmount = 2f;
    public int damageAmount = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (dangerSound && SFXManager.Instance != null)
                SFXManager.Instance.PlaySFX3D(dangerSound, transform.position);

            if (effectOnHit)
                Instantiate(effectOnHit, transform.position, Quaternion.identity);

            CarController car = other.GetComponent<CarController>();
            if (car != null)
            {
                
                car.TakeDamage(damageAmount);

                Rigidbody rb = other.GetComponent<Rigidbody>();

                Debug.Log("Car flipped and damaged!");

                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;

                    other.transform.position += Vector3.up * liftAmount;

                    Vector3 rot = other.transform.eulerAngles;
                    rot.z = (rot.z + 180f) % 360f;
                    other.transform.eulerAngles = rot;

                    rb.AddForce(Vector3.up * flipForce, ForceMode.Impulse);
                    rb.AddTorque(Vector3.right * torqueForce, ForceMode.Impulse);
                }
            }

            Destroy(gameObject);
        }
    }
}
