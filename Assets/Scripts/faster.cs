using UnityEngine;

public class faster : MonoBehaviour
{
    public float boostMultiplier = 3000;      
    public float boostDuration = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("BOOST dla gracza na " + boostDuration + "s");

            CarController car = other.GetComponent<CarController>();
            if (car != null)
            {
                car.StartCoroutine(car.BoostMassAndSpeed(650f, 400f, boostDuration));
            }
        }
        else if (other.CompareTag("AI"))
        {
            Debug.Log("BOOST dla AI na " + boostDuration + "s");

            AIController ai = other.GetComponent<AIController>();
            if (ai != null)
            {
                ai.StartCoroutine(ai.BoostMassAndSpeed(20f, 20f, boostDuration));
            }
        }
    }




}
