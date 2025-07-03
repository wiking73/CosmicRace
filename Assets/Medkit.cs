using UnityEngine;

public class Medkit : MonoBehaviour
{
    public int healAmount = 30;
    public AudioClip healSound;
    public GameObject effectOnPickup;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (healSound && SFXManager.Instance != null)
                SFXManager.Instance.PlaySFX3D(healSound, transform.position);

            if (effectOnPickup)
                Instantiate(effectOnPickup, transform.position, Quaternion.identity);

            CarController car = other.GetComponent<CarController>();
            if (car != null)
            {
                car.Heal(healAmount);
            }

            Destroy(gameObject);
        }
    }
}

