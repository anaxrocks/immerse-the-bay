using UnityEngine;
using System.Collections.Generic;

public class SpotlightTrigger : MonoBehaviour
{
    public float maxDistance = 10f;
    public LayerMask ghostCubeMask;
    public LayerMask ghostMask;  // NEW: Add this for detecting ghosts
    public GameObject ghostPrefab;
    public int maxGhosts = 5;

    private GhostCube lastCubeHit;
    private List<Ghost> activeGhosts = new List<Ghost>();

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // -------------------------
        // 1. Spawn ghosts from cubes
        // -------------------------
        if (Physics.Raycast(ray, out hit, maxDistance, ghostCubeMask))
        {
            GhostCube cube = hit.collider.GetComponent<GhostCube>();

            if (cube != null && cube != lastCubeHit)
            {
                lastCubeHit = cube;

                if (activeGhosts.Count < maxGhosts)
                {
                    GameObject g = Instantiate(ghostPrefab, cube.spawnPoint.position, cube.spawnPoint.rotation);
                    activeGhosts.Add(g.GetComponent<Ghost>());
                    Debug.Log("Spawned ghost!");
                }
                else
                {
                    Debug.Log("Max ghosts reached!");
                }
            }
        }
        else
        {
            lastCubeHit = null;
        }

        // -------------------------
        // 2. Handle ghost visibility with GHOST LAYER MASK
        // -------------------------
        RaycastHit ghostHit;

        if (Physics.Raycast(ray, out ghostHit, maxDistance, ghostMask))  // NOW USES ghostMask
        {
            Ghost ghost = ghostHit.collider.GetComponentInParent<Ghost>();
            if (ghost != null)
            {
                Debug.Log($"Spotlight hitting ghost: {ghost.gameObject.name}");
                ghost.MarkSeen();
            }
        }

        // Clean nulls
        activeGhosts.RemoveAll(g => g == null);
    }
}