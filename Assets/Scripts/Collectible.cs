using UnityEngine;

public class Collectible : MonoBehaviour
{
    public AudioClip collectSound;
    public GameObject effectOnCollect; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") ) 
        {
            if (collectSound)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

            if (effectOnCollect)
                Instantiate(effectOnCollect, transform.position, Quaternion.identity);

            Destroy(gameObject); 
        }
    }
}

