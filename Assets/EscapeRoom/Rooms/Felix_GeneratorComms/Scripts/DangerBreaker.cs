using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DangerBreaker : MonoBehaviour
{
    [Header("Interaction")]
    public XRSimpleInteractable interactable;

    [Header("Breaker Visual")]
    public Transform breakerVisual;

    [Header("Rotation")]
    public Vector3 onEuler;
    public Vector3 offEuler;

    [Header("Fault Feedback")]
    public ParticleSystem sparkFX;
    public AudioSource zapAudio;

    [Header("Puzzle")]
    public FuseBoxPuzzle controller;

    private bool busy = false;

    private void Start()
    {
        if (interactable == null)
            interactable = GetComponent<XRSimpleInteractable>();

        if (interactable != null)
            interactable.selectEntered.AddListener(OnSelected);

        if (breakerVisual != null)
            breakerVisual.localEulerAngles = offEuler;
    }

    private void OnDestroy()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnSelected);
    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        if (!busy)
            StartCoroutine(FaultRoutine());
    }

    private IEnumerator FaultRoutine()
    {
        busy = true;

        // Red breaker briefly flips ON
        if (breakerVisual != null)
            breakerVisual.localEulerAngles = onEuler;

        if (sparkFX != null)
            sparkFX.Play();

        if (zapAudio != null)
            zapAudio.Play();

        yield return new WaitForSeconds(0.35f);

        // Reset entire fuse panel
        if (controller != null)
            controller.ResetAllBreakers();

        // Red breaker returns OFF too
        if (breakerVisual != null)
            breakerVisual.localEulerAngles = offEuler;

        busy = false;
    }
}