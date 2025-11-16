using Unity.VisualScripting;
using UnityEngine;

public class NetCatch : MonoBehaviour
{
    public Transform netHoldPoint;
    public AudioClip fullNetAudio;
    public AudioSource audioSource;

    [HideInInspector] public Ghost currentGhost;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ghost"))
        {
            Ghost ghost = other.GetComponent<Ghost>();
            if (ghost == null) return;

            ProximitySphere ps = ghost.GetComponent<ProximitySphere>();
            if (ps != null && !ps.dialogueCompleted) return;

            if (currentGhost == null)
            {
                CatchGhost(ghost);
            }
            else
            {
                if (!audioSource.isPlaying && fullNetAudio && ghost != currentGhost)
                {
                    audioSource.PlayOneShot(fullNetAudio);
                }
            }
        }
    }

    void CatchGhost(Ghost ghost)
    {
        currentGhost = ghost;
        ghost.GoIntoNet(netHoldPoint);
    }

    public void ReleaseGhostToHeart(Transform heartAttachPoint)
    {
        if (currentGhost != null)
        {
            currentGhost.AttachToHeart(heartAttachPoint);
            currentGhost = null;
        }
    }
}
