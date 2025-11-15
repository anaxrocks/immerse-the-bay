using UnityEngine;
using Oculus.Voice.Dictation;   // AppDictationExperience lives here

public class CollisionDictation : MonoBehaviour
{
    public AppDictationExperience dictation;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dictation.Activate();     // Start recording
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dictation.Deactivate();   // Stop recording
        }
    }
}
