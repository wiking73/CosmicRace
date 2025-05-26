using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Wjecha� obiekt: " + other.name);
        if (other.name.StartsWith("Player") || other.name.StartsWith("AI"))
        {
            RaceManager.Instance.FinishRace(other.gameObject);
            Debug.Log(other.name + " uko�czy� wy�cig!");
        }
    }
}
