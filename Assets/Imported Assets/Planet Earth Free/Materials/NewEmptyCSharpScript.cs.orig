using UnityEngine;

public class NewEmptyCSharpScript: MonoBehaviour
{
    public float delay = 3f;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        Invoke("Drop", delay);
    }

    void Drop()
    {
        rb.isKinematic = false;
    }
}

