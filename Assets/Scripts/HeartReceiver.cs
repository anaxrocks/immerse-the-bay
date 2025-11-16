using UnityEngine;

public class HeartReceiver : MonoBehaviour
{
    public Transform ghostAttachPoint; // The empty child transform

    private void OnTriggerEnter(Collider other)
    {
        NetCatch net = other.GetComponent<NetCatch>();

        if (net != null && net.currentGhost != null)
        {
            // Release ghost onto the heart's child attach point
            net.ReleaseGhostToHeart(ghostAttachPoint);
        }
    }
}
