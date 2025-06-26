using UnityEngine;

public class Danger : MonoBehaviour
{
    public AudioClip dangerSound;
    public GameObject effectOnHit;

    public float flipForce = 5f;
    public float torqueForce = 3f;
    public float liftAmount = 2f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (dangerSound)
                AudioSource.PlayClipAtPoint(dangerSound, transform.position);

            if (effectOnHit)
                Instantiate(effectOnHit, transform.position, Quaternion.identity);

            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Debug.Log("Car flipped!");

                
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                
                other.transform.position += Vector3.up * liftAmount;

               
                Vector3 rot = other.transform.eulerAngles;
                rot.x = (rot.x + 180f) % 360f;
                other.transform.eulerAngles = rot;

                
                rb.AddForce(Vector3.up * flipForce, ForceMode.Impulse);
                rb.AddTorque(Vector3.right * torqueForce, ForceMode.Impulse);
            }

            Destroy(gameObject);
        }
    }
}
