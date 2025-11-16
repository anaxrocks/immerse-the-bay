using UnityEngine;

public class SpotlightTrigger : MonoBehaviour
{
    public float maxDistance = 10f;
    public LayerMask ghostCubeMask;
    public GameObject ghostPrefab;

    private GhostCube lastCubeHit;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, ghostCubeMask))
        {
            GhostCube cube = hit.collider.GetComponent<GhostCube>();

            if (cube != null && cube != lastCubeHit)
            {
                lastCubeHit = cube; // prevent double-triggering

                // spawn at object's own spawnPoint
                Instantiate(ghostPrefab, cube.spawnPoint.position, cube.spawnPoint.rotation);
                Debug.Log("Spawned ghost at cube's spawn point!");
                Destroy(cube);
            }
        }
        else
        {
            // reset so you can trigger again
            lastCubeHit = null;
        }
    }
}
