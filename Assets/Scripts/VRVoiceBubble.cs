using UnityEngine;
using TMPro;

public class VRBubbleText : MonoBehaviour
{
    public TextMeshProUGUI speechText;

    // This function will be called by the Dictation prefab event
    public void UpdateBubble(string transcription)
    {
        if (speechText != null)
            speechText.text = transcription;
    }
}
