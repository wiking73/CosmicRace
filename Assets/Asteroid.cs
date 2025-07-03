using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public float fallSpeed = 2f;
    public int damageAmount = 20;
    public GameObject smokeEffectPrefab;

    void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Uderzenie w ziemiê
        if (other.CompareTag("Untagged"))
        {
            if (smokeEffectPrefab != null)
                Instantiate(smokeEffectPrefab, transform.position, Quaternion.identity);

            
        }

        // Uderzenie w gracza
        CarController car = other.GetComponent<CarController>();
        if (car != null)
        {
            car.TakeDamage(damageAmount);

            if (smokeEffectPrefab != null)
                Instantiate(smokeEffectPrefab, transform.position, Quaternion.identity);

            
        }
    }


}
