using UnityEngine;
using System.Collections.Generic;

public class ProximitySphere : MonoBehaviour
{
    [Header("Proximity Settings")]
    public float detectionRadius = 3f;
    
    [Header("Dialogue Configuration")]
    public string[] dialogueLines = new string[]
    {
        "Carrot cake is sweet… but only sweeter with someone who truly gets me.", 
        "Do you like long walks… or are you more of a hop-and-skip type?",  
        "My sketches are lonely without a heart that colors outside the lines with me.", 
        "Notice the little joys… or I might have to steal them all for myself!", 
        "Not just anyone can share my frosting… only the ones who understand patience.",  
        "Can your heart giggle at nonsense but still feel deeply when it matters?",
        "Some paths are too ordinary… let's wander where magic hides.",
        "Love is like baking: it takes care, attention, and a pinch of sweetness.",
        "Do you blush easily? Because I might… just a little, if you're worthy.",
        "Some days are for doodles, some days are for daydreams… are you up for both?"  
    };
    
    [Header("Dialogue Settings")]
    public float dialogueCooldown = 4f; // Seconds between dialogue changes
    public float fadeDuration = 0.5f; // Seconds for fade in/out
    public float pauseDuration = 1f; // Seconds to wait between fade out and fade in
    
    private float lastDialogueTime = -999f;
    private string currentDialogue = "";
    private string nextDialogue = "";
    private bool isPlayerInProximity = false;
    
    private enum FadeState { Idle, FadingOut, Paused, FadingIn }
    private FadeState fadeState = FadeState.Idle;
    private float stateStartTime = 0f;
    
    private List<string> remainingDialogues;
    private float currentAlpha = 0f;
    
    void Start()
    {
        // Initialize the dialogue pool
        ResetDialoguePool();
    }
    
    void ResetDialoguePool()
    {
        remainingDialogues = new List<string>(dialogueLines);
        ShuffleList(remainingDialogues);
    }
    
    void ShuffleList(List<string> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            string temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
    
    string GetNextDialogue()
    {
        if (remainingDialogues.Count == 0)
        {
            ResetDialoguePool();
        }
        
        string dialogue = remainingDialogues[0];
        remainingDialogues.RemoveAt(0);
        return dialogue;
    }
    
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
                    // First time entering proximity
                    if (!isPlayerInProximity)
                    {
                        isPlayerInProximity = true;
                        currentDialogue = GetNextDialogue();
                        lastDialogueTime = Time.time;
                        fadeState = FadeState.FadingIn;
                        stateStartTime = Time.time;
                        currentAlpha = 0f;
                        Debug.Log($"Selected dialogue: {currentDialogue}");
                    }
                    // Check if it's time to change dialogue
                    else if (fadeState == FadeState.Idle && Time.time - lastDialogueTime >= dialogueCooldown)
                    {
                        // Start fade out sequence
                        nextDialogue = GetNextDialogue();
                        fadeState = FadeState.FadingOut;
                        stateStartTime = Time.time;
                    }
                    
                    // Handle fade state machine
                    float stateTime = Time.time - stateStartTime;
                    
                    switch (fadeState)
                    {
                        case FadeState.FadingOut:
                            currentAlpha = Mathf.Lerp(1f, 0f, stateTime / fadeDuration);
                            if (stateTime >= fadeDuration)
                            {
                                currentAlpha = 0f;
                                fadeState = FadeState.Paused;
                                stateStartTime = Time.time;
                                Debug.Log("Text hidden - pausing");
                            }
                            break;
                            
                        case FadeState.Paused:
                            currentAlpha = 0f; // Keep hidden
                            if (stateTime >= pauseDuration)
                            {
                                // NOW switch the text while invisible
                                currentDialogue = nextDialogue;
                                lastDialogueTime = Time.time;
                                fadeState = FadeState.FadingIn;
                                stateStartTime = Time.time;
                                Debug.Log($"Switching to new dialogue: {currentDialogue}");
                            }
                            break;
                            
                        case FadeState.FadingIn:
                            currentAlpha = Mathf.Lerp(0f, 1f, stateTime / fadeDuration);
                            if (stateTime >= fadeDuration)
                            {
                                currentAlpha = 1f;
                                fadeState = FadeState.Idle;
                                Debug.Log("Text fully visible");
                            }
                            break;
                            
                        case FadeState.Idle:
                            currentAlpha = 1f;
                            break;
                    }
                    
                    // Register current dialogue
                    if (!string.IsNullOrEmpty(currentDialogue))
                    {
                        ProximityManager.Instance.RegisterProximity(distance, currentDialogue);
                        ProximityManager.Instance.SetDialogueAlpha(currentAlpha);
                    }
                }
                else
                {
                    // Player left proximity
                    if (isPlayerInProximity)
                    {
                        isPlayerInProximity = false;
                        fadeState = FadeState.Idle;
                        currentAlpha = 0f;
                    }
                }
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}