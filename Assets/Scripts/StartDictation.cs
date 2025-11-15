using UnityEngine;
using Meta.WitAi.Dictation;
using Meta.WitAi;                // ← required in newer Meta SDKs
using Oculus.Voice.Dictation;    // ← required for Dictation Building Block


public class StartDictation : MonoBehaviour
{
    public AppDictationExperience dictation;

    public void BeginDictation()
    {
        dictation.Activate();
    }

    public void StopDictation()
    {
        dictation.Deactivate();
    }
}
