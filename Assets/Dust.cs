using UnityEngine;

public class DustEmitter : MonoBehaviour
{
    public ParticleSystem dustParticles;
    public Rigidbody vehicleRb;
    public float speedThreshold = 1f;

    void Update()
    {
        if (vehicleRb.linearVelocity.magnitude > speedThreshold)
        {
            if (!dustParticles.isPlaying)
                dustParticles.Play();
        }
        else
        {
            if (dustParticles.isPlaying)
                dustParticles.Stop();
        }
    }
}
