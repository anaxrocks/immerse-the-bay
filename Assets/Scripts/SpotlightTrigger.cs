using UnityEngine;
using System.Collections.Generic;

public class SpotlightTrigger : MonoBehaviour
{
    public Transform targetToFace;   // assign the player/camera here

    public float maxDistance = 10f;
    public LayerMask ghostCubeMask;
    public LayerMask ghostMask;

    public GameObject[] ghostPrefabs;   // NEW: list of ghost types
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
                    GameObject prefab = GetRandomGhostPrefab();
                    if (prefab != null)
                    {
                       GameObject g = Instantiate(prefab, cube.spawnPoint.position, Quaternion.identity);

                        if (targetToFace != null)
                        {
                            Vector3 lookPos = targetToFace.position;
                            lookPos.y = g.transform.position.y; // keep upright
                            g.transform.LookAt(lookPos);
                        }
                        else
                        {
                            Debug.LogWarning("targetToFace not assigned!");
                        }


                        activeGhosts.Add(g.GetComponent<Ghost>());
                        Debug.Log("Spawned ghost: " + g.name);

                        // Despawn ghost cube
                        Destroy(cube.gameObject);
                        lastCubeHit = null;
                    }
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

        if (Physics.Raycast(ray, out ghostHit, maxDistance, ghostMask))
        {
            Ghost ghost = ghostHit.collider.GetComponentInParent<Ghost>();
            if (ghost != null)
            {
                ghost.MarkSeen();
            }
        }

        // Clean up null references
        activeGhosts.RemoveAll(g => g == null);
    }

    // -------------------------
    // Select random ghost prefab
    // -------------------------
    private GameObject GetRandomGhostPrefab()
    {
        if (ghostPrefabs == null || ghostPrefabs.Length == 0)
        {
            Debug.LogError("No ghost prefabs assigned in SpotlightTrigger!");
            return null;
        }

        return ghostPrefabs[Random.Range(0, ghostPrefabs.Length)];
    }
}
