using Unity.VisualScripting;
using UnityEngine;

public class GhostCube : MonoBehaviour
{
    public Transform spawnPoint;


void Start() {
    spawnPoint  = gameObject.transform;
}
}