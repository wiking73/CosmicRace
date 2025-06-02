using UnityEngine;

public class faster : MonoBehaviour
{
    public float boostMultiplier = 3000;      
    public float boostDuration = 2f;        

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("BOOST activated!");
            CarController car = other.GetComponent<CarController>();
            if (car != null)
            {
                car.StartCoroutine(car.ActivateBoost(boostMultiplier, boostDuration));
            }
        }
    }
    
}
