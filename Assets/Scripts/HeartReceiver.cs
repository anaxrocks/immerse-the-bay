using UnityEngine;

public class HeartReceiver : MonoBehaviour
{
    public GameObject correctGhostPrefab; // the correct answer (ghost_bow)

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
            if (!isMatched)
            {
                isMatched = true;
                print("Ghost matched!");
                matchAudio.Play();

                Matchmaking.Instance.SetCurrentGhosts(mainGhost, net.currentGhost);
                net.ReleaseGhostToHeart(ghostAttachPoint);

                if (IsCorrectGhost(net.currentGhost.gameObject))
                {
                    Matchmaking.Instance.MakeMatchAccepted();
                }
                else
                {
                    Matchmaking.Instance.MakeMatchRejected();
                }
            }
        }
    }

    private bool IsCorrectGhost(GameObject ghostObj)
    {
        return ghostObj.name.Contains(correctGhostPrefab.name);
    }
}
