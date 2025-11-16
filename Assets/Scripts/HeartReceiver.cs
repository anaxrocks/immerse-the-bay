using UnityEngine;

public class HeartReceiver : MonoBehaviour
{
    public Transform ghostAttachPoint; // The empty child transform
    public AudioSource matchAudio;
    public bool isMatched = false;
    ParticleSystem heartParticles;
    public Ghost mainGhost;
    private void OnTriggerEnter(Collider other)
    {
        NetCatch net = other.GetComponent<NetCatch>();

        if (net != null && net.currentGhost != null)
        {
            // Release ghost onto the heart's child attach point
            
             if (!isMatched)
        {
            isMatched = true;
            print("Ghost matched!");
            matchAudio.Play();
            // heartParticles.Play();
            Matchmaking.Instance.SetCurrentGhosts(mainGhost, net.currentGhost);
            net.ReleaseGhostToHeart(ghostAttachPoint);
            
        }
        }
    }
}
