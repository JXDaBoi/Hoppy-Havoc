using UnityEngine;

public class Audio_Jump : MonoBehaviour
{
    public AudioSource JumpAudio_1;
    public AudioSource JumpAudio_2;

    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    public void PlayJumpSound()
    {
        JumpAudio_1.pitch = Random.Range(minPitch, maxPitch);
        JumpAudio_1.Play();
    }

    public void PlayRunJumpSound()
    {
        JumpAudio_2.pitch = Random.Range(minPitch, maxPitch);
        JumpAudio_2.Play();
    }
}
