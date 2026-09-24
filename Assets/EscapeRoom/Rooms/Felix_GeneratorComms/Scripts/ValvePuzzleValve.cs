using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ValvePuzzleValve : MonoBehaviour
{
    [Header("Sequence")]
    public ValveSequenceController controller;

    [Header("Colors")]
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;
    public Color hoverColor = new Color(0.1f, 0.7f, 1f);

    [Header("Hover")]
    public GameObject hoverIndicator;
    public float hoverIntensity = 0.7f;

    [Header("Result Feedback")]
    public GameObject resultIndicator;
    public float resultIntensity = 1.2f;

    private XRSimpleInteractable interactable;

    private Material hoverMaterial;
    private Material resultMaterial;

    private bool activated = false;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip turnSound;
    public AudioClip correctSound;
    public AudioClip wrongSound;

    [Header("Progression")]
    public bool startEnabled = false;

    public float feedbackSoundDelay = 0.35f;

    void Start()
    {
        interactable = GetComponent<XRSimpleInteractable>();

        if (interactable != null)
        {
            interactable.hoverEntered.AddListener(OnHoverEnter);
            interactable.hoverExited.AddListener(OnHoverExit);
            interactable.selectEntered.AddListener(OnGrabbed);
        }

        if (hoverIndicator != null)
        {
            Renderer hoverRenderer =
                hoverIndicator.GetComponent<Renderer>();

            if (hoverRenderer != null)
            {
                hoverMaterial = hoverRenderer.material;

                hoverMaterial.EnableKeyword("_EMISSION");
                hoverMaterial.SetColor(
                    "_EmissionColor",
                    hoverColor * hoverIntensity
                );

                hoverMaterial.color = hoverColor;
            }

            hoverIndicator.SetActive(false);
        }

        if (resultIndicator != null)
        {
            Renderer resultRenderer =
                resultIndicator.GetComponent<Renderer>();

            if (resultRenderer != null)
                resultMaterial = resultRenderer.material;

            resultIndicator.SetActive(false);
        }

        if (interactable != null)
            interactable.enabled = startEnabled;
    }

    private IEnumerator PlayFeedbackSoundDelayed(AudioClip clip)
    {
        yield return new WaitForSeconds(feedbackSoundDelay);

        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    public void SetInteractionEnabled(bool enabled)
    {
        if (interactable != null)
            interactable.enabled = enabled;

        if (!enabled && hoverIndicator != null)
            hoverIndicator.SetActive(false);
    }

    private void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(OnHoverEnter);
            interactable.hoverExited.RemoveListener(OnHoverExit);
            interactable.selectEntered.RemoveListener(OnGrabbed);
        }
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        if (hoverIndicator != null)
            hoverIndicator.SetActive(true);
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        if (hoverIndicator != null)
            hoverIndicator.SetActive(false);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {

        if (audioSource != null && turnSound != null)
            audioSource.PlayOneShot(turnSound);

        if (activated)
            return;

        activated = true;

        if (hoverIndicator != null)
            hoverIndicator.SetActive(false);

        if (controller != null)
            controller.ValveTurned(this);
    }

    public void ShowCorrect()
    {
        ShowResult(correctColor);

        if (interactable != null)
            interactable.enabled = false;

        if (correctSound != null)
            StartCoroutine(PlayFeedbackSoundDelayed(correctSound));
    }

    public void ShowWrong()
    {
        ShowResult(wrongColor);

        if (wrongSound != null)
            StartCoroutine(PlayFeedbackSoundDelayed(wrongSound));
    }

    private void ShowResult(Color color)
    {
        if (resultIndicator == null || resultMaterial == null)
            return;

        resultIndicator.SetActive(true);

        resultMaterial.EnableKeyword("_EMISSION");
        resultMaterial.SetColor(
            "_EmissionColor",
            color * resultIntensity
        );

        resultMaterial.color = color;
    }

    public void ResetValve()
    {
        activated = false;

        if (interactable != null)
            interactable.enabled = true;

        if (resultIndicator != null)
            resultIndicator.SetActive(false);

        if (hoverIndicator != null)
            hoverIndicator.SetActive(false);
    }
}