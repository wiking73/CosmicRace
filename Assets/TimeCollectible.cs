using UnityEngine;

public class TimeCollectible : MonoBehaviour
{
    public AudioClip collectSound;
    public GameObject effectOnCollect;
    public float bonusTime = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") )
        {
            
            if (collectSound && SFXManager.Instance != null)
                SFXManager.Instance.PlaySFX3D(collectSound, transform.position);

            if (effectOnCollect)
                Instantiate(effectOnCollect, transform.position, Quaternion.identity);

           
            TimeCounter counter = FindObjectOfType<TimeCounter>();
            if (counter != null)
            {
                counter.AddTime(bonusTime);
            }

            
            Destroy(gameObject);
        }
    }
}
