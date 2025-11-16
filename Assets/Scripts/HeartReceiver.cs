using Unity.VisualScripting;
using UnityEngine;

public class HeartReceiver : MonoBehaviour
{
    public GameObject correctGhostPrefab; // the correct answer (ghost_bow)

    public Transform ghostAttachPoint; // The empty child transform
    public AudioSource matchAudio;
    public bool isMatched = false;
    ParticleSystem heartParticles;
    public GameObject mainGhost;
    public GameObject currentGhost;
    private void OnTriggerEnter(Collider other)
    {
        NetCatch net = other.GetComponent<NetCatch>();

        if (net != null && net.currentGhost != null)
        {
            if (!isMatched)
            {
                isMatched = true;
                print("Ghost matched!");

                Matchmaking.Instance.SetCurrentGhosts(mainGhost.GetComponent<Ghost>(), net.currentGhost);
                currentGhost = net.currentGhost.gameObject;
                net.ReleaseGhostToHeart(ghostAttachPoint);

                if (IsCorrectGhost(currentGhost.gameObject))
                {
                    Matchmaking.Instance.MakeMatchAccepted();
                    matchAudio.Play();
                }
                else
                {
                    Matchmaking.Instance.MakeMatchRejected();
                    matchAudio.Play();
                }
            }
        }
    }

    private bool IsCorrectGhost(GameObject ghostObj)
    {
        return ghostObj.name.Contains(correctGhostPrefab.name);
    }
}
