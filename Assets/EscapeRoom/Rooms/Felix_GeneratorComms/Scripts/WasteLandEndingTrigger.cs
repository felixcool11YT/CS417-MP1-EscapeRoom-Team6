using UnityEngine;
using TMPro;
using System.Collections;

public class WastelandEnding : MonoBehaviour
{
    [Header("Ending UI")]
    public CanvasGroup whiteFlash;
    public GameObject endingTextObject;

    [Header("Timing")]
    public float flashInTime = 0.15f;
    public float holdWhiteTime = 0.35f;
    public float flashOutTime = 1.5f;
    public float textDelay = 0.75f;

    [Header("Audio")]
    public AudioSource endingAudio;

    private bool triggered = false;

    [Header("VR UI")]
    public Transform endingCanvas;
    public float canvasDistance = 0.5f;


    private void Start()
    {
        if (whiteFlash != null)
            whiteFlash.alpha = 0f;

        if (endingTextObject != null)
            endingTextObject.SetActive(false);

        Camera cam = Camera.main;

        if (cam != null && endingCanvas != null)
        {
            endingCanvas.SetParent(cam.transform);

            endingCanvas.localPosition =
                new Vector3(0f, 0f, canvasDistance);

            endingCanvas.localRotation =
                Quaternion.identity;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (other.GetComponentInParent<CharacterController>() == null)
            return;

        triggered = true;
        StartCoroutine(PlayEnding());
    }

    private IEnumerator PlayEnding()
    {
        if (endingAudio != null)
            endingAudio.Play();

        if (whiteFlash != null)
        {
            float elapsed = 0f;

            while (elapsed < flashInTime)
            {
                elapsed += Time.deltaTime;
                whiteFlash.alpha = Mathf.Lerp(
                    0f,
                    1f,
                    elapsed / flashInTime
                );

                yield return null;
            }

            whiteFlash.alpha = 1f;

            yield return new WaitForSeconds(holdWhiteTime);

            elapsed = 0f;

            while (elapsed < flashOutTime)
            {
                elapsed += Time.deltaTime;
                whiteFlash.alpha = Mathf.Lerp(
                    1f,
                    0f,
                    elapsed / flashOutTime
                );

                yield return null;
            }

            whiteFlash.alpha = 0f;
        }

        yield return new WaitForSeconds(textDelay);

        if (endingTextObject != null)
            endingTextObject.SetActive(true);

        Debug.Log("WASTELAND ENDING TRIGGERED");
    }
}