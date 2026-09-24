using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRSocketInteractor))]
public class FinalDoorSocket : MonoBehaviour
{
    [SerializeField] private FinalDoorController doorController;
    [SerializeField] private Transform feedbackVisual;
    [SerializeField] private Renderer feedbackRenderer;
    [SerializeField] private Light statusLight;
    [SerializeField] private Color acceptedColor = Color.green;
    [SerializeField] private float feedbackDuration = 0.6f;

    private XRSocketInteractor socketInteractor;
    private bool itemRegistered;

    private void Awake()
    {
        socketInteractor = GetComponent<XRSocketInteractor>();
    }

    private void OnEnable()
    {
        socketInteractor.selectEntered.AddListener(OnItemInserted);
    }

    private void OnDisable()
    {
        socketInteractor.selectEntered.RemoveListener(OnItemInserted);
    }

    private void OnItemInserted(SelectEnterEventArgs args)
    {
        if (itemRegistered || doorController == null)
            return;

        itemRegistered = true;
        doorController.RegisterItem();
        StartCoroutine(PlayAcceptedFeedback());

        XRGrabInteractable insertedItem =
            args.interactableObject.transform.GetComponent<XRGrabInteractable>();

        if (insertedItem != null)
            insertedItem.enabled = false;
    }

    private IEnumerator PlayAcceptedFeedback()
    {
        if (feedbackVisual == null)
            yield break;

        Vector3 startingScale = feedbackVisual.localScale;
        Material feedbackMaterial = feedbackRenderer != null ? feedbackRenderer.material : null;
        Color startingColor = feedbackMaterial != null && feedbackMaterial.HasProperty("_EmissionColor")
            ? feedbackMaterial.GetColor("_EmissionColor")
            : Color.black;

        if (statusLight != null)
        {
            statusLight.color = acceptedColor;
            statusLight.enabled = true;
        }

        float elapsed = 0f;
        while (elapsed < feedbackDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / feedbackDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            float pulse = 1f + Mathf.Sin(progress * Mathf.PI) * 0.15f;

            feedbackVisual.localScale = startingScale * pulse;

            if (feedbackMaterial != null && feedbackMaterial.HasProperty("_EmissionColor"))
                feedbackMaterial.SetColor("_EmissionColor",
                    Color.Lerp(startingColor, acceptedColor * 2f, easedProgress));

            yield return null;
        }

        feedbackVisual.localScale = startingScale;

        if (feedbackMaterial != null && feedbackMaterial.HasProperty("_EmissionColor"))
            feedbackMaterial.SetColor("_EmissionColor", acceptedColor * 2f);
    }
}
