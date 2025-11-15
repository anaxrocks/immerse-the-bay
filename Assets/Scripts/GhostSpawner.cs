using UnityEngine;

public class GhostSpawner : MonoBehaviour
{
    public GameObject ghostPrefab;

    public void SpawnGhost(Vector3 pos)
    {
        Instantiate(ghostPrefab, pos, Quaternion.identity);
    }
}
