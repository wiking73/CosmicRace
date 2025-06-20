using UnityEngine;

public class Lights : MonoBehaviour
{
    public Light[] swiatla; 
    public float delay = 0.5f; 

    private int current = 0;

    void Start()
    {
        StartCoroutine(Migaj());
    }

    System.Collections.IEnumerator Migaj()
    {
        while (true)
        {
            
            foreach (var swiatlo in swiatla)
                swiatlo.enabled = false;

           
            swiatla[current].enabled = true;

            
            current = (current + 1) % swiatla.Length;

            yield return new WaitForSeconds(delay);
        }
    }
}
