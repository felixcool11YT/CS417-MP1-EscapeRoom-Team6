using UnityEngine;

public class ValvePuzzleSuccessFeedback : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip successSound;
    public ParticleSystem successParticles;

    public void PlaySuccess()
    {
        if (audioSource != null && successSound != null)
            audioSource.PlayOneShot(successSound);

        if (successParticles != null)
            successParticles.Play();
    }
}
