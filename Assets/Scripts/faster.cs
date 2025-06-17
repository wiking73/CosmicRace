using UnityEngine;

public class faster : MonoBehaviour
{
    public float boostMultiplier = 3000;      
    public float boostDuration = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("BOOST for " + boostDuration + "s");

            CarController car = other.GetComponent<CarController>();
            if (car != null)
            {
                car.StartCoroutine(car.BoostMassAndSpeed(650f, 400f, boostDuration));
            }
        }
    }



}
