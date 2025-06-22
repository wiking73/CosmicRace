using UnityEngine;

public class nextWaypoint : MonoBehaviour
{
    public float freezeDuration = 1.5f;
    public GameObject teleportEffect;
    public AudioClip droneSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CarController car = other.GetComponent<CarController>();
            if (car != null)
            {
                if (teleportEffect)
                    Instantiate(teleportEffect, other.transform.position, Quaternion.identity);

                if (droneSound)
                    AudioSource.PlayClipAtPoint(droneSound, other.transform.position);

                car.StartCoroutine(car.FreezeAndTeleport(freezeDuration));
            }

            Destroy(gameObject); 
        }
    }
}
