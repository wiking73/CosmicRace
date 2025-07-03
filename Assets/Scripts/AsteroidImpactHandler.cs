using UnityEngine;

public class AsteroidImpactHandler : MonoBehaviour
{
    public GameObject impactEffectPrefab;

    void OnCollisionEnter(Collision collision)
    {
        // SprawdŸ, czy asteroida uderzy³a w "ziemiê" lub inny du¿y obiekt
        // Mo¿esz dodaæ tagi do pod³ogi/terenu, np. "Ground"
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Player"))
        {
            // Opcjonalnie: odtwarzanie dŸwiêku uderzenia
            // AudioSource.PlayClipAtPoint(impactSound, transform.position);


            if (impactEffectPrefab != null)
            {
                GameObject impact = Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
                Destroy(impact, 2f); 

            }

            Destroy(gameObject);
        }
    }
}