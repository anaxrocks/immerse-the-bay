using UnityEngine;
using TMPro;

public class SpeechBubble : MonoBehaviour
{
    public TextMeshProUGUI textBox; // assign your text box here
    public string[] messages;        // array of messages to cycle through
    public float interval = 2f;      // time between messages in seconds

    private int index = 0;

    void Start()
    {
        if(messages.Length > 0)
            textBox.text = messages[0];

        InvokeRepeating(nameof(UpdateText), interval, interval);
    }

    void UpdateText()
    {
        if (messages.Length == 0) return;

        index = (index + 1) % messages.Length;
        textBox.text = messages[index];
    }
}
