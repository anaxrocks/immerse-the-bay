using UnityEngine;
using TMPro;
using System.Collections;

public class Matchmaking : MonoBehaviour
{
    public static Matchmaking Instance;
    [Header("Life System")]
    public int lives = 3;
    public GameObject[] lifeObjects; // Assign 3 life GameObjects in inspector
    
    [Header("Match Success")]
    public GameObject heartAnimation; // Particle system or animated heart
    public TextMeshProUGUI matchSuccessText;
    public string successMessage = "Perfect Match!";
    public float textDisplayDuration = 2f;
    public float floatUpSpeed = 2f;
    public float floatUpDuration = 2f;
    
    private Ghost currentMainGhost;
    private Ghost currentCaughtGhost;
    public HeartReceiver heartReceiver;
    
    void Start()
    {
        Instance = this;
        if (matchSuccessText != null)
            matchSuccessText.gameObject.SetActive(false);
            
        if (heartAnimation != null)
            heartAnimation.SetActive(false);
            
        UpdateLivesDisplay();
    }

    public void SetCurrentGhosts(Ghost mainGhost, Ghost caughtGhost)
    {
        currentMainGhost = mainGhost;
        currentCaughtGhost = caughtGhost;
    }

    public void MakeMatchAccepted()
    {
        if (currentMainGhost != null && currentCaughtGhost != null)
        {
            StartCoroutine(PlayMatchSuccessSequence());
        }
    }

    public void MakeMatchRejected()
    {
        heartReceiver.isMatched = false;
        lives -= 1;
        UpdateLivesDisplay();
        
        // Release the caught ghost back 
        if (currentCaughtGhost != null)
        {
            // Destroy
            StartCoroutine(FadeAndDestroy(currentCaughtGhost.gameObject));
            currentCaughtGhost = null;
        }
        
        // Check for game over
        if (lives <= 0)
        {
            GameOver();
        }
    }
    
    private IEnumerator PlayMatchSuccessSequence()
    {
        // Play heart animation
        if (heartAnimation != null)
        {
            heartAnimation.SetActive(true);
            ParticleSystem ps = heartAnimation.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();
        }
        
        // Float both ghosts upward
        if (currentMainGhost != null && currentCaughtGhost != null)
        {
            StartCoroutine(FloatGhostUp(currentMainGhost.transform));
            StartCoroutine(FloatGhostUp(currentCaughtGhost.transform));
        }
        
        // Wait a moment, then show text
        yield return new WaitForSeconds(floatUpDuration * 0.5f);
        
        if (matchSuccessText != null)
        {
            matchSuccessText.text = successMessage;
            matchSuccessText.gameObject.SetActive(true);
        }
        
        // Wait for text display
        yield return new WaitForSeconds(textDisplayDuration);
        
        // Hide text
        if (matchSuccessText != null)
        {
            matchSuccessText.gameObject.SetActive(false);
        }
        
        // Hide heart animation
        if (heartAnimation != null)
        {
            heartAnimation.SetActive(false);
        }
        
        // Destroy both ghosts
        if (currentMainGhost != null)
            Destroy(currentMainGhost.gameObject);
        if (currentCaughtGhost != null)
            Destroy(currentCaughtGhost.gameObject);
            
        currentMainGhost = null;
        currentCaughtGhost = null;
    }
    
    private IEnumerator FloatGhostUp(Transform ghostTransform)
    {
        float elapsed = 0f;
        Vector3 startPos = ghostTransform.position;
        
        while (elapsed < floatUpDuration)
        {
            elapsed += Time.deltaTime;
            float distance = floatUpSpeed * Time.deltaTime;
            ghostTransform.position += Vector3.up * distance;
            yield return null;
        }
    }
    
    private void UpdateLivesDisplay()
    {
        if (lifeObjects == null || lifeObjects.Length == 0) return;
        
        for (int i = 0; i < lifeObjects.Length; i++)
        {
            if (lifeObjects[i] != null)
            {
                lifeObjects[i].SetActive(i < lives);
            }
        }
    }
    
    private IEnumerator FadeAndDestroy(GameObject rootGhost, float duration = 1.5f)
{
    if (rootGhost == null) yield break;

    // Get all renderers in ghost + children
    Renderer[] renderers = rootGhost.GetComponentsInChildren<Renderer>();
    float elapsed = 0f;

    // Cache original colors
    Material[][] materials = new Material[renderers.Length][];
    Color[][] originalColors = new Color[renderers.Length][];

    for (int i = 0; i < renderers.Length; i++)
    {
        materials[i] = renderers[i].materials;
        originalColors[i] = new Color[materials[i].Length];

        for (int m = 0; m < materials[i].Length; m++)
        {
            originalColors[i][m] = materials[i][m].color;

            // IMPORTANT: enable fade mode so alpha works
            materials[i][m].SetFloat("_Surface", 1); // URP Lit = Transparent
            materials[i][m].SetFloat("_Blend", 0);
            materials[i][m].renderQueue = 3000;
        }
    }

    // Fade out
    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float t = elapsed / duration;

        for (int r = 0; r < renderers.Length; r++)
        {
            for (int m = 0; m < materials[r].Length; m++)
            {
                Color c = originalColors[r][m];
                c.a = Mathf.Lerp(1f, 0f, t);
                materials[r][m].color = c;
            }
        }

        yield return null;
    }

    Destroy(rootGhost);
}


    private void GameOver()
    {
        Debug.Log("Game Over!");
        // Add your game over logic here
    }
}