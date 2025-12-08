using UnityEngine;

public class BlockParticles : MonoBehaviour
{
    public ParticleSystem blockBreakParticles;
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 2f)
        {
            if (blockBreakParticles != null)
            {
                ParticleSystem particles = Instantiate(blockBreakParticles, collision.contacts[0].point, Quaternion.identity);
                Destroy(particles.gameObject, 2f);
            }
        }
    }
}