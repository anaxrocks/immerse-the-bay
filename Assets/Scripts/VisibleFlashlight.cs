using UnityEngine;

public class VisibleOnlyInFlashlight : MonoBehaviour
{
    public Light flashlight;          // assign your spotlight in Inspector
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.enabled = false;        // start invisible
    }

    void Update()
    {

        // vector from flashlight to particle system
        Vector3 toObject = transform.position - flashlight.transform.position;

        // angle between flashlight forward direction and the object
        float angle = Vector3.Angle(flashlight.transform.forward, toObject);

        bool insideCone = angle < (flashlight.spotAngle / 2f);
        bool insideRange = toObject.magnitude < flashlight.range;

        // visible ONLY when inside light cone + in range
        rend.enabled = insideCone && insideRange;
    }
}
