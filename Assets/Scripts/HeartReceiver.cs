using UnityEngine;

public class HeartReceiver : MonoBehaviour
{
    public Transform ghostAttachPoint; // The empty child transform
 public AudioSource matchAudio;
    bool isMatched = false;
    ParticleSystem heartParticles;
    private void OnTriggerEnter(Collider other)
    {
        NetCatch net = other.GetComponent<NetCatch>();

        if (net != null && net.currentGhost != null)
        {
            // Release ghost onto the heart's child attach point
            net.ReleaseGhostToHeart(ghostAttachPoint);
             if (!isMatched)
        {
            isMatched = true;
            print("Ghost matched!");
            matchAudio.Play();
            // heartParticles.Play();
        }
        }
    }
}
