using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float smoothSpeed = 10f;
    private bool isInNet = false;
    private Transform target;

    private void Update()
    {
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
        target = holdPoint;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    public void AttachToHeart(Transform heart)
    {
        isInNet = false;
        target = null;
        transform.SetParent(heart);
        transform.position = heart.position;
        transform.rotation = heart.rotation;

        GetComponent<Rigidbody>().isKinematic = true;
    }
}
