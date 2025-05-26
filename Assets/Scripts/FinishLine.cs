using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.name.StartsWith("AI"))
        {
            RaceManager.Instance.FinishRace(other.name);
            Debug.Log(other.name + " ukoñczy³ wyœcig!");
        }
    }
}
