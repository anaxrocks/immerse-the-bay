using UnityEngine;

public class RandomPrefabSpawnerWithHeight : MonoBehaviour
{
    [Header("Player Target")]
    public Transform player;            

    [Header("Prefabs To Spawn")]
    public GameObject[] prefabs;        

    [Header("Spawn Amount")]
    public int amountToSpawn = 10;      

    [Header("Horizontal Distance")]
    public float minDistance = 2f;      
    public float maxDistance = 6f;      

    [Header("Vertical Height Range")]
    public float minHeight = -1f;       // below player
    public float maxHeight = 2.5f;      // above player

    [Header("Random Rotation")]
    public bool randomRotation = true;

    [Header("Debug")]
    public bool spawnOnStart = true;

    void Start()
    {
        if (spawnOnStart)
            SpawnAll();
    }

    public void SpawnAll()
    {
        for (int i = 0; i < amountToSpawn; i++)
        {
            SpawnOne();
        }
    }

    public void SpawnOne()
    {
        if (player == null)
        {
            Debug.LogError("Player not assigned!");
            return;
        }

        if (prefabs.Length == 0)
        {
            Debug.LogError("No prefabs assigned!");
            return;
        }

        // Pick a random prefab
        GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];

        // Horizontal random angle & distance
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Random.Range(minDistance, maxDistance);

        Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * distance;

        // Height offset (random Y)
        float randomHeight = Random.Range(minHeight, maxHeight);

        // Final spawn position
        Vector3 spawnPos = player.position + offset;
        spawnPos.y += randomHeight;

        // Random or default rotation
        Quaternion rot = randomRotation ? Random.rotation : Quaternion.identity;

        Instantiate(prefab, spawnPos, rot);
    }
}
