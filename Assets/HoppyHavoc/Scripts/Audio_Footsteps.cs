using UnityEngine;

public class Audio_Footsteps : MonoBehaviour
{
    public AudioSource audioSource;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    public void PlayFootstepSound()
    {
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.Play();
    }
}
