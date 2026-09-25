using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    [Header("Button")]
    [SerializeField] private float pressDistance = 0.02f;
    [SerializeField] private float pressDuration = 0.1f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pressSound;

    private Vector3 originalPosition;
    private bool isPressed;

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    public void PressButton()
    {
        if (isPressed)
            return;

        StartCoroutine(PressAndRestart());
    }

    private IEnumerator PressAndRestart()
    {
        isPressed = true;

        // Move button inward along its local Y-axis.
        Vector3 pressedPosition =
            originalPosition + Vector3.down * pressDistance;

        float elapsed = 0f;

        while (elapsed < pressDuration)
        {
            transform.localPosition = Vector3.Lerp(
                originalPosition,
                pressedPosition,
                elapsed / pressDuration
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = pressedPosition;

        // Play button sound.
        if (audioSource != null && pressSound != null)
        {
            audioSource.PlayOneShot(pressSound);
        }

        // Brief delay so the player sees/hears the button press.
        yield return new WaitForSeconds(0.25f);

        // Reload the current scene.
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}