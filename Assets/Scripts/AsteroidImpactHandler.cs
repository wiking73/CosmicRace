using UnityEngine;

public class AsteroidImpactHandler : MonoBehaviour
{
    public GameObject impactEffectPrefab; // <--- PRZYWRÓĆ TO POLE
    public AudioClip impactSound;
    [Range(0.0f, 1.0f)]
    public float impactSoundVolume = 1.0f;

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"AsteroidImpactHandler: Asteroid collided with: {collision.gameObject.name}. Layer: {LayerMask.LayerToName(collision.gameObject.layer)}");

        Rigidbody otherRigidbody = collision.gameObject.GetComponent<Rigidbody>();
        bool isStationary = (otherRigidbody == null || otherRigidbody.isKinematic);
        Debug.Log($"AsteroidImpactHandler: Is stationary object: {isStationary}. Other Rigidbody: {otherRigidbody}. IsKinematic: {(otherRigidbody != null ? otherRigidbody.isKinematic.ToString() : "N/A")}");

        if (isStationary)
        {
            Debug.Log($"AsteroidImpactHandler: Collision with a stationary object ({collision.gameObject.name}). Proceeding to play sound and effect.");

            if (impactSound != null)
            {
                AudioSource.PlayClipAtPoint(impactSound, transform.position, impactSoundVolume);
                Debug.Log($"AsteroidImpactHandler: Played impact sound '{impactSound.name}' at {transform.position} with volume {impactSoundVolume}.");
            }
            else
            {
                Debug.LogWarning("AsteroidImpactHandler: impactSound is null. No impact sound will be played.");
            }

            // ODTWARZANIE EFEKTU, JEŚLI PRZYWRÓCONY
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
        }
        else
        {
            Debug.Log($"AsteroidImpactHandler: Collision with a MOVING object ({collision.gameObject.name}). No sound/effect for dynamic collision.");
        }
    }
}