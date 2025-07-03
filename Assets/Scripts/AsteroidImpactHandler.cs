using UnityEngine;

public class AsteroidImpactHandler : MonoBehaviour
{
    public GameObject impactEffectPrefab;

    void OnCollisionEnter(Collision collision)
    {
        // Sprawd�, czy asteroida uderzy�a w "ziemi�" lub inny du�y obiekt
        // Mo�esz doda� tagi do pod�ogi/terenu, np. "Ground"
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Player"))
        {
            // Opcjonalnie: odtwarzanie d�wi�ku uderzenia
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