using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float fadeDelay = 3f;

    private float timeSinceSeen = 0f;
    private bool isSeen = false;
    private bool childrenActive = false;

    private bool isInNet = false;
    private bool hasBeenCaught = false;  // NEW: Track if ghost was ever caught
    private Transform target;
    public float smoothSpeed = 10f;

    void Start()
    {
        SetChildrenActive(false); // start invisible
    }

    private void Update()
    {
        // Only fade if ghost has never been caught
        if (!isSeen && !hasBeenCaught)
        {
            timeSinceSeen += Time.deltaTime;

            if (timeSinceSeen >= fadeDelay && childrenActive)
            {
                Debug.Log($"Hiding ghost: {gameObject.name}");
                SetChildrenActive(false);  // hide children
            }
        }

        isSeen = false; // must be reset each frame

        // If caught, smoothly follow the hold point
        if (isInNet && target != null)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * smoothSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * smoothSpeed);
        }
    }

    public void GoIntoNet(Transform holdPoint)
    {
        isInNet = true;
        hasBeenCaught = true;  // NEW: Mark as caught permanently
        target = holdPoint;
        GetComponent<Rigidbody>().isKinematic = true;
        
        // Make sure ghost is visible when caught
        SetChildrenActive(true);
    }

    public void AttachToHeart(Transform heart)
    {
        isInNet = false;
        target = null;

        transform.SetParent(heart);
        transform.position = heart.position;
        transform.rotation = heart.rotation;

        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().enabled = false;
        
        // Keep ghost visible
        SetChildrenActive(true);
    }

    public void MarkSeen()
    {
        isSeen = true;
        timeSinceSeen = 0f;

        // If ghost is hidden, show it
        if (!childrenActive)
        {
            Debug.Log($"Showing ghost: {gameObject.name}");
            SetChildrenActive(true);
        }
    }

    private void SetChildrenActive(bool active)
    {
        childrenActive = active;

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(active);
        }
    }
}