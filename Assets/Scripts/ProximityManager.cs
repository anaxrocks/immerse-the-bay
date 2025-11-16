using UnityEngine;
using TMPro;

public class ProximityManager : MonoBehaviour
{
    public static ProximityManager Instance;
    public TextMeshProUGUI proximityText;
    
    [Header("Fade Settings")]
    public float fadeStartDistance = 5f;
    public float fadeEndDistance = 10f;
    public float fadeSpeed = 5f;
    
    private Transform playerTransform;
    private string currentText = "";
    private string lastText = "";
    private float closestDistance = Mathf.Infinity;
    private CanvasGroup canvasGroup;
    private float targetAlpha = 0f;
    private bool hasActiveProximity = false;
    
    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        playerTransform = Camera.main.transform;

        if (proximityText != null)
        {
            canvasGroup = proximityText.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = proximityText.gameObject.AddComponent<CanvasGroup>();

            canvasGroup.alpha = 0f;
        }
    }
    
    void LateUpdate()
    {
        hasActiveProximity = !string.IsNullOrEmpty(currentText);

        if (hasActiveProximity)
        {
            lastText = currentText;
        }

        if (canvasGroup != null)
        {
            // Calculate target alpha based on distance
            if (hasActiveProximity)
            {
                if (closestDistance <= fadeStartDistance)
                    targetAlpha = 1f;
                else if (closestDistance >= fadeEndDistance)
                    targetAlpha = 0f;
                else
                    targetAlpha = 1f - (closestDistance - fadeStartDistance) / (fadeEndDistance - fadeStartDistance);
            }
            else
            {
                targetAlpha = 0f;
            }

            // Smooth fade
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
        }

        // Update text content
        if (proximityText != null)
            proximityText.text = lastText;

        currentText = "";
        closestDistance = Mathf.Infinity;
    }
    
    public void RegisterProximity(float distance, string text)
    {
        if (distance < closestDistance)
        {
            closestDistance = distance;
            currentText = text;
        }
    }

    public Transform GetPlayerTransform()
    {
        return playerTransform;
    }
}
