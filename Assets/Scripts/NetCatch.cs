using UnityEngine;

public class NetCatch : MonoBehaviour
{
    public Transform netHoldPoint; // where ghost stick to
    public AudioClip fullNetAudio; // warning audio
    public AudioSource audioSource;

    [HideInInspector] public Ghost currentGhost;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("[x] COLLIDED");
        if (other.CompareTag("ghost"))
        {
            Ghost ghost = other.GetComponent<Ghost>();
            if (ghost == null) return;

            if (currentGhost == null)
            {
                // First ghost → catch it!
                CatchGhost(ghost);
            }
            else
            {
                // Net already has a ghost → play error sound
                if (audioSource && fullNetAudio)
                    audioSource.PlayOneShot(fullNetAudio);
            }
        }
    }

    void CatchGhost(Ghost ghost)
    {
        Debug.Log("CAUGHT");
        currentGhost = ghost;
        ghost.GoIntoNet(netHoldPoint);
    }

    public void ReleaseGhostToHeart(Transform heartTransform)
    {
        if (currentGhost != null)
        {
            currentGhost.AttachToHeart(heartTransform);
            currentGhost = null;
        }
    }
}
