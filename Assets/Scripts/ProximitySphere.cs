using UnityEngine;
using System.Collections.Generic;

public class ProximitySphere : MonoBehaviour
{
    [Header("Proximity Settings")]
    public float detectionRadius = 3f;
    
    [Header("Dialogue Configuration")]
    public string[] dialogueLines;
    
    [Header("Dialogue Settings")]
    public float dialogueCooldown = 4f;
    public float fadeDuration = 0.5f;
    public float pauseDuration = 1f;

    public bool dialogueCompleted = false;

    private float lastDialogueTime = -999f;
    private string currentDialogue = "";
    private string nextDialogue = "";
    private bool isPlayerInProximity = false;

    private enum FadeState { Idle, FadingOut, Paused, FadingIn }
    private FadeState fadeState = FadeState.Idle;
    private float stateStartTime = 0f;

    private List<string> remainingDialogues;
    private int dialoguesShown = 0;

    private float currentAlpha = 0f;

    private Ghost ghost;

    void Start()
    {
        ghost = GetComponent<Ghost>();
        ResetDialoguePool();
    }
    
    void ResetDialoguePool()
    {
        remainingDialogues = new List<string>(dialogueLines);
    }
    
    string GetNextDialogue()
    {
        if (remainingDialogues.Count == 0)
        {
            ResetDialoguePool();
        }

        string dialogue = remainingDialogues[0];
        remainingDialogues.RemoveAt(0);

        dialoguesShown++;
        if (dialoguesShown >= dialogueLines.Length)
        {
            dialogueCompleted = true;
        }

        return dialogue;
    }
    
    void Update()
    {
        if (ProximityManager.Instance == null || ghost == null)
            return;

        Transform playerTransform = ProximityManager.Instance.GetPlayerTransform();
        if (playerTransform == null)
            return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= detectionRadius)
        {
            // Only run dialogue if ghost is visible
            if (ghost.childrenActive)
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
                        }
                        break;
                        
                    case FadeState.FadingIn:
                        currentAlpha = Mathf.Lerp(0f, 1f, stateTime / fadeDuration);
                        if (stateTime >= fadeDuration)
                        {
                            currentAlpha = 1f;
                            fadeState = FadeState.Idle;
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
                // Ghost is hidden → pause dialogue
                fadeState = FadeState.Idle;
                currentAlpha = 0f;
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
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
