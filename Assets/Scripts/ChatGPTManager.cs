using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class ChatMessage
{
    public string role;
    public string content;
}

[System.Serializable]
public class ChatRequest
{
    public string model;
    public List<ChatMessage> messages;
    public float temperature = 0.7f;
    public int max_tokens = 1000;
}

[System.Serializable]
public class ChatResponse
{
    public string id;
    public string @object;
    public int created;
    public Choice[] choices;
    public Usage usage;
}

[System.Serializable]
public class Choice
{
    public int index;
    public ChatMessage message;
    public string finish_reason;
}

[System.Serializable]
public class Usage
{
    public int prompt_tokens;
    public int completion_tokens;
    public int total_tokens;
}

public class ChatGPTManager : MonoBehaviour
{
    [Header("API Configuration")]
    [SerializeField] private string apiKey = "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";
    [SerializeField] private string model = "gpt-4o-mini";
    
    private const string API_URL = "https://api.openai.com/v1/chat/completions";
    
    private List<ChatMessage> conversationHistory = new List<ChatMessage>();
    
    public void SendMessage(string userMessage, Action<string> onSuccess, Action<string> onError)
    {
        StartCoroutine(SendMessageCoroutine(userMessage, onSuccess, onError));
    }
    
    // New method for one-off dialogue generation without conversation history
    public void GenerateDialogue(string systemPrompt, string userPrompt, Action<string> onSuccess, Action<string> onError)
    {
        StartCoroutine(GenerateDialogueCoroutine(systemPrompt, userPrompt, onSuccess, onError));
    }
    
    private IEnumerator GenerateDialogueCoroutine(string systemPrompt, string userPrompt, Action<string> onSuccess, Action<string> onError)
    {
        // Create temporary message list for this dialogue generation
        List<ChatMessage> messages = new List<ChatMessage>();
        
        if (!string.IsNullOrEmpty(systemPrompt))
        {
            messages.Add(new ChatMessage
            {
                role = "system",
                content = systemPrompt
            });
        }
        
        messages.Add(new ChatMessage
        {
            role = "user",
            content = userPrompt
        });
        
        // Create request
        ChatRequest request = new ChatRequest
        {
            model = model,
            messages = messages,
            temperature = 0.9f, // Higher temperature for more creative responses
            max_tokens = 25
        };
        
        string jsonData = JsonUtility.ToJson(request);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest www = new UnityWebRequest(API_URL, "POST");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", $"Bearer {apiKey}");
        
        yield return www.SendWebRequest();
        
        if (www.result == UnityWebRequest.Result.Success)
        {
            try
            {
                ChatResponse response = JsonUtility.FromJson<ChatResponse>(www.downloadHandler.text);
                string assistantMessage = response.choices[0].message.content;
                onSuccess?.Invoke(assistantMessage);
            }
            catch (Exception e)
            {
                onError?.Invoke($"Parse error: {e.Message}");
            }
        }
        else
        {
            onError?.Invoke($"API Error: {www.error}\n{www.downloadHandler.text}");
        }
        
        www.Dispose();
    }
    
    private IEnumerator SendMessageCoroutine(string userMessage, Action<string> onSuccess, Action<string> onError)
    {
        // Add user message to history
        conversationHistory.Add(new ChatMessage
        {
            role = "user",
            content = userMessage
        });
        
        // Create request
        ChatRequest request = new ChatRequest
        {
            model = model,
            messages = new List<ChatMessage>(conversationHistory),
            temperature = 0.7f,
            max_tokens = 1000
        };
        
        string jsonData = JsonUtility.ToJson(request);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest www = new UnityWebRequest(API_URL, "POST");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", $"Bearer {apiKey}");
        
        yield return www.SendWebRequest();
        
        if (www.result == UnityWebRequest.Result.Success)
        {
            try
            {
                ChatResponse response = JsonUtility.FromJson<ChatResponse>(www.downloadHandler.text);
                string assistantMessage = response.choices[0].message.content;
                
                // Add assistant response to history
                conversationHistory.Add(new ChatMessage
                {
                    role = "assistant",
                    content = assistantMessage
                });
                
                onSuccess?.Invoke(assistantMessage);
            }
            catch (Exception e)
            {
                onError?.Invoke($"Parse error: {e.Message}");
            }
        }
        else
        {
            onError?.Invoke($"API Error: {www.error}\n{www.downloadHandler.text}");
        }
        
        www.Dispose();
    }
    
    public void ClearConversation()
    {
        conversationHistory.Clear();
    }
    
    public void SetSystemMessage(string systemMessage)
    {
        conversationHistory.Clear();
        conversationHistory.Add(new ChatMessage
        {
            role = "system",
            content = systemMessage
        });
    }
}