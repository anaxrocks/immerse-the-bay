using UnityEngine;
using System.Collections.Generic;

public class SpotlightTrigger : MonoBehaviour
{
    public Transform targetToFace;

    public float maxDistance = 10f;
    public LayerMask ghostCubeMask;
    public LayerMask ghostMask;

    public GameObject[] ghostPrefabs;

    private GhostCube lastCubeHit;

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

                GameObject prefab = GetRandomGhostPrefab();
                if (prefab != null)
                {
                    GameObject g = Instantiate(prefab, cube.spawnPoint.position, Quaternion.identity);

                    // Face the player
                    if (targetToFace != null)
                    {
                        Vector3 lookPos = targetToFace.position;
                        lookPos.y = g.transform.position.y; // keep upright
                        g.transform.LookAt(lookPos);
                        g.transform.Rotate(0, -90f, 0); // rotation fix for your prefab facing +X
                    }
                    else
                    {
                        Debug.LogWarning("targetToFace not assigned!");
                    }

                    Debug.Log("Spawned ghost: " + g.name);

                    // Despawn the cube
                    Destroy(cube.gameObject);
                    lastCubeHit = null;
                }
            }
        }
        else
        {
            lastCubeHit = null;
        }

        // -------------------------
        // 2. Ghost visibility via GHOST MASK
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
    }

    // -------------------------
    // Random ghost prefab
    // -------------------------
    private GameObject GetRandomGhostPrefab()
    {
        if (ghostPrefabs == null || ghostPrefabs.Length == 0)
        {
            Debug.LogError("No ghost prefabs assigned!");
            return null;
        }

        return ghostPrefabs[Random.Range(0, ghostPrefabs.Length)];
    }
}
