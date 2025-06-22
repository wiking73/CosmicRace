using UnityEngine;

public class Collectible : MonoBehaviour
{
    public AudioClip collectSound;
    public GameObject effectOnCollect;

    public float boostMass = 650f;
    public float boostSpeed = 400f;
    public float boostDuration = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           
            if (collectSound)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

            
            if (effectOnCollect)
                Instantiate(effectOnCollect, transform.position, Quaternion.identity);

           
            CarController car = other.GetComponent<CarController>();
            if (car != null)
            {
                Debug.Log("COLLECTIBLE BOOST for " + boostDuration + "s");
                car.StartCoroutine(car.BoostMassAndSpeed(boostMass, boostSpeed, boostDuration));
            }

            Destroy(gameObject);
        }
    }
}
