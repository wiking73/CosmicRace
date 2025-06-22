using UnityEngine;

public class stars : MonoBehaviour
{
    public AudioClip collectSound;
    public GameObject effectOnCollect;
    public int pointsToAdd = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (collectSound)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

            if (effectOnCollect)
                Instantiate(effectOnCollect, transform.position, Quaternion.identity);

            
            Scores.Instance.AddPoints(pointsToAdd);

            Destroy(gameObject);
        }
    }
}
