using UnityEngine;

public class RockDrop : MonoBehaviour
{
     public float delay = 2f;

    void Start()
    {
        Invoke("Drop", delay);
    }

    void Drop()
    {
        GetComponent<Rigidbody>().isKinematic = false;
    }
}
