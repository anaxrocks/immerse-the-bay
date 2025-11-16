using UnityEngine;
using System.Collections.Generic;

public class ProximitySphere : MonoBehaviour
{
    [Header("Proximity Settings")]
    public float detectionRadius = 3f;
    
    [Header("Dialogue Configuration")]
    public string[] dialogueLines;   // Editable in inspector
    
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
                    else if (fadeState == FadeState.Idle && Time.time - lastDialogueTime >= dialogueCooldown)
                    {
                        nextDialogue = GetNextDialogue();
                        fadeState = FadeState.FadingOut;
                        stateStartTime = Time.time;
                    }
                    
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
                            currentAlpha = 0f;
                            if (stateTime >= pauseDuration)
                            {
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
                    
                    if (!string.IsNullOrEmpty(currentDialogue))
                    {
                        ProximityManager.Instance.RegisterProximity(distance, currentDialogue);
                        ProximityManager.Instance.SetDialogueAlpha(currentAlpha);
                    }
                }
                else
                {
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
