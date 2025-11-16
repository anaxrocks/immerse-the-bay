using UnityEngine;

public class HeartReceiver : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        NetCatch net = other.GetComponent<NetCatch>();
        if (net != null && net.currentGhost != null)
        {
            // Heart receives the ghost
            net.ReleaseGhostToHeart(transform);
        }
    }
}
