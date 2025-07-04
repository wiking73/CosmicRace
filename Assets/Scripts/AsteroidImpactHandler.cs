using UnityEngine;

public class AsteroidImpactHandler : MonoBehaviour
{
    public GameObject impactEffectPrefab; // <--- PRZYWRÓĆ TO POLE
    public AudioClip impactSound;
    [Range(0.0f, 1.0f)]
    public float impactSoundVolume = 1.0f;
    [Header("Player Impact Properties")]
    public int damageAmount = 20;public AudioClip playerHitSound; // Dźwięk uderzenia w gracza (opcjonalnie, może być ten sam co impactSound)
    [Range(0.0f, 1.0f)]
    public float playerHitSoundVolume = 1.0f; 


    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"AsteroidImpactHandler: Asteroid collided with: {collision.gameObject.name}. Layer: {LayerMask.LayerToName(collision.gameObject.layer)}. Speed: {collision.relativeVelocity.magnitude}");

        // --- Logika uderzenia w GRACZA ---
        if (collision.gameObject.CompareTag("Player"))
        {
            CarController car = collision.gameObject.GetComponent<CarController>();
            if (car != null)
            {
                car.TakeDamage(damageAmount);
                Debug.Log($"AsteroidImpactHandler: Asteroid hit Player ({collision.gameObject.name}) and dealt {damageAmount} damage. ");

                // Odtwórz dźwięk uderzenia w gracza
                if (playerHitSound != null)
                {
                    // Używamy AudioSource.PlayClipAtPoint, które jest jednorazowe i nie wymaga AudioSource na obiekcie
                    AudioSource.PlayClipAtPoint(playerHitSound, transform.position, playerHitSoundVolume);
                }
                else if (impactSound != null) // Jeśli nie ma dedykowanego dźwięku dla gracza, użyj ogólnego
                {
                    AudioSource.PlayClipAtPoint(impactSound, transform.position, impactSoundVolume);
                }
                else
                {
                    Debug.LogWarning("AsteroidImpactHandler: No playerHitSound or impactSound assigned. No sound played on player hit.");
                }

                // Opcjonalnie: Zniszcz asteroidę po uderzeniu w gracza?
                // Jeśli asteroida ma znikać po uderzeniu w gracza, odkomentuj poniższą linię:
                // Destroy(gameObject); 
                return; // Zakończ, bo już obsłużyliśmy kolizję z graczem
            }
        }

        // --- Logika uderzenia w STATYCZNY obiekt (ziemia, ściany itp.) ---
        Rigidbody otherRigidbody = collision.gameObject.GetComponent<Rigidbody>();
        bool isStationary = (otherRigidbody == null || otherRigidbody.isKinematic);

        if (isStationary)
        {
            Debug.Log($"AsteroidImpactHandler: Collision with a stationary object ({collision.gameObject.name}). Proceeding to play sound and effect.");

            // Odtwarzanie dźwięku uderzenia w statyczny obiekt
            if (impactSound != null)
            {
                AudioSource.PlayClipAtPoint(impactSound, transform.position, impactSoundVolume);
                Debug.Log($"AsteroidImpactHandler: Played impact sound '{impactSound.name}' at {transform.position} with volume {impactSoundVolume}.");
            }
            else
            {
                Debug.LogWarning("AsteroidImpactHandler: impactSound is null. No impact sound will be played.");
            }

            // Odtwarzanie efektu uderzenia
            if (impactEffectPrefab != null)
            {
                GameObject impact = Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
                Debug.Log($"AsteroidImpactHandler: Instantiated impact effect at {transform.position}.");
                Destroy(impact, 2f); 
            }
            else
            {
                Debug.LogWarning("AsteroidImpactHandler: impactEffectPrefab is null. No impact effect will be played.");
            }
            
            // WAŻNE: Asteroida nie jest niszczona (chyba że zdecydujesz inaczej dla uderzenia w gracza)
        }
        else
        {
            Debug.Log($"AsteroidImpactHandler: Collision with a MOVING object ({collision.gameObject.name}). No specific sound/effect for dynamic collision (other than player).");
        }
    }
}