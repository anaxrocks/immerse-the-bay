using UnityEngine;

public class ProximitySphere : MonoBehaviour
{
    public float detectionRadius = 3f;
    public string nearText = "You are near the sphere!";
    
    void Update()
    {
        if (ProximityManager.Instance != null)
        {
            Transform playerTransform = ProximityManager.Instance.GetPlayerTransform();
            
            if (playerTransform != null)
            {
                float distance = Vector3.Distance(transform.position, playerTransform.position);
                
                if (distance <= detectionRadius)
                {
                    ProximityManager.Instance.RegisterProximity(distance, nearText);
                }
            }
        }
    }
}