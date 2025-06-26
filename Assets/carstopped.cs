using UnityEngine;

public class carstopped : MonoBehaviour
{
    public float freezeDuration = 2f;
    public GameObject trapEffect;
    public AudioClip trapSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CarController car = other.GetComponent<CarController>();
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
