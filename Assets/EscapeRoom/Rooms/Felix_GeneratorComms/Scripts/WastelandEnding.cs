using UnityEngine;
using TMPro;
using System.Collections;

public class WastelandEnding : MonoBehaviour
{
    [Header("Departure interlock")]
    [SerializeField] private DepartureStationController departureStation;
    [SerializeField] private BunkerHatchController hatch;
    [SerializeField] private GameFlowController gameFlow;

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
    private BoxCollider exitVolume;
    private Camera playerCamera;

    [Header("VR UI")]
    public Transform endingCanvas;
    public float canvasDistance = 0.5f;


    private void Start()
    {
        exitVolume = GetComponent<BoxCollider>();
        playerCamera = Camera.main;
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

    private void OnTriggerEnter(Collider other) => TryEscape(other);
    private void OnTriggerStay(Collider other) => TryEscape(other);

    private void Update()
    {
        if (!playerCamera || !exitVolume || !exitVolume.enabled || triggered)
            return;

        // The tracked head can enter the exit before the locomotion capsule does.
        Vector3 point = exitVolume.transform.InverseTransformPoint(playerCamera.transform.position);
        Vector3 half = exitVolume.size * 0.5f;
        Vector3 offset = point - exitVolume.center;
        float headroom = 2f / Mathf.Max(Mathf.Abs(exitVolume.transform.lossyScale.y), 0.001f);
        if (Mathf.Abs(offset.x) <= half.x && Mathf.Abs(offset.z) <= half.z
            && offset.y >= -half.y && offset.y <= half.y + headroom)
            CompleteEscape();
    }

    private void TryEscape(Collider other)
    {
        if (other.GetComponentInParent<CharacterController>() != null)
            CompleteEscape();
    }

    private void CompleteEscape()
    {
        if (triggered || departureStation == null || !departureStation.IsComplete
            || hatch == null || !hatch.IsOpen || gameFlow == null || gameFlow.HasEnded)
            return;

        triggered = true;
        gameFlow.CompleteGame();
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