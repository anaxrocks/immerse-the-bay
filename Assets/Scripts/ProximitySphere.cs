using UnityEngine;

public class ProximitySphere : MonoBehaviour
{
    [Header("Proximity Settings")]
    public float detectionRadius = 3f;
    
    [Header("Persona Configuration")]
    [TextArea(3, 10)]
    public string personaDescription = "You are a girl who wants to find love. You like the outdoors and picnics";
    
    [Header("Dialogue Settings")]
    public float dialogueCooldown = 5f; // Minimum seconds between dialogue generations
    
    private ChatGPTManager chatGPTManager;
    private float lastDialogueTime = -999f;
    private bool isGeneratingDialogue = false;
    private string currentDialogue = "";
    
    void Start()
    {
        chatGPTManager = FindObjectOfType<ChatGPTManager>();
        if (chatGPTManager == null)
        {
            Debug.LogError("ChatGPTManager not found! Please add it to the scene.");
        }
    }
    
    void Update()
    {
        if (ProximityManager.Instance != null && chatGPTManager != null)
        {
            Transform playerTransform = ProximityManager.Instance.GetPlayerTransform();
            if (playerTransform != null)
            {
                float distance = Vector3.Distance(transform.position, playerTransform.position);
                if (distance <= detectionRadius)
                {
                    // Check if enough time has passed and we're not already generating
                    if (!isGeneratingDialogue && Time.time - lastDialogueTime > dialogueCooldown)
                    {
                        GenerateNewDialogue();
                    }
                    
                    // Register current dialogue (whether new or existing)
                    if (!string.IsNullOrEmpty(currentDialogue))
                    {
                        ProximityManager.Instance.RegisterProximity(distance, currentDialogue);
                    }
                }
            }
        }
    }
    
    void GenerateNewDialogue()
    {
        isGeneratingDialogue = true;
        lastDialogueTime = Time.time;
        
        string prompt = "Generate a short greeting or comment (1 sentence) that this character would say when someone approaches. Be creative and vary your responses.";
        
        chatGPTManager.GenerateDialogue(
            systemPrompt: personaDescription,
            userPrompt: prompt,
            onSuccess: (response) =>
            {
                currentDialogue = response.Trim();
                isGeneratingDialogue = false;
                Debug.Log($"Generated dialogue: {currentDialogue}");
            },
            onError: (error) =>
            {
                currentDialogue = "..."; // Fallback text
                isGeneratingDialogue = false;
                Debug.LogError($"Dialogue generation failed: {error}");
            }
        );
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}