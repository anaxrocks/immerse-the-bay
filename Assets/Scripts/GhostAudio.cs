using UnityEngine;
using System.Collections;

public class GhostAudio : MonoBehaviour
{
    public float soundRadius = 6f;
    public float minDelay = 3f;
    public float maxDelay = 8f;

    public AudioClip[] ghostSounds;
    private AudioSource audioSource;
    private Transform player;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;  // 3D sound
        player = GameObject.FindWithTag("Player").transform;

        StartCoroutine(PlayRandomSounds());
    }

    IEnumerator PlayRandomSounds()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));

            if (Vector3.Distance(player.position, transform.position) <= soundRadius)
            {
                if (ghostSounds.Length > 0)
                {
                    audioSource.clip = ghostSounds[Random.Range(0, ghostSounds.Length)];
                    audioSource.Play();
                }
            }
        }
    }
}
