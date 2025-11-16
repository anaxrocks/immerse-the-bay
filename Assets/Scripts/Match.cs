using UnityEngine;

public class Match : MonoBehaviour
{
    public AudioSource matchAudio;
    bool isMatched = false;
    ParticleSystem heartParticles;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        // also have a ghost in net check?
        if (other.gameObject.CompareTag("ghost") && !isMatched)
        {
            isMatched = true;
            print("Ghost matched!");
            matchAudio.Play();
            // heartParticles.Play();
        }
    }
}
