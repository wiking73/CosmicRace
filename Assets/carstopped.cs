using UnityEngine;

public class carstopped : MonoBehaviour
{
    public float freezeDuration = 2f;
    public GameObject trapEffect;
    public AudioClip trapSound;
    public int damageAmount = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CarController car = other.GetComponent<CarController>();
            car.TakeDamage(damageAmount);
            if (car != null)
            {
                if (trapEffect)
                    Instantiate(trapEffect, other.transform.position, Quaternion.identity);

                if (trapSound && SFXManager.Instance != null)
                    SFXManager.Instance.PlaySFX3D(trapSound, other.transform.position); 

                car.StartCoroutine(car.FreezeCar(freezeDuration));
            }

            Destroy(gameObject); 
        }
    }
}
