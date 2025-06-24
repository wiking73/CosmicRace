using UnityEngine;

public class stopcars : MonoBehaviour
{
    public float stopDuration = 3f;
    public AudioClip stopSound;
    public GameObject stopEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (stopSound)
                AudioSource.PlayClipAtPoint(stopSound, transform.position);

            if (stopEffect)
                Instantiate(stopEffect, transform.position, Quaternion.identity);

            
            foreach (GameObject ai in GameObject.FindGameObjectsWithTag("AI"))
            {
                AIController controller = ai.GetComponent<AIController>();
                if (controller != null)
                    controller.StartCoroutine(controller.StopTemporarily(stopDuration));
            }

            Destroy(gameObject);
        }
    }
}
