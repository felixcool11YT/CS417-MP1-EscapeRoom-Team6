using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

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
        SecureItem(args.interactableObject as XRGrabInteractable);
        doorController.RegisterItem();
        StartCoroutine(PlayAcceptedFeedback());
    }

    private void SecureItem(XRGrabInteractable item)
    {
        if (item == null) return;
        var itemAttach = item.GetAttachTransform(socketInteractor);
        var socketAttach = socketInteractor.GetAttachTransform(item);
        item.transform.rotation = socketAttach.rotation
            * Quaternion.Inverse(itemAttach.rotation) * item.transform.rotation;
        item.transform.position += socketAttach.position - itemAttach.position;
        var position = item.transform.position;
        var rotation = item.transform.rotation;
        // Selection precedes socket smoothing. Keep the seated pose when release
        // callbacks run, then secure the installed clearance against impacts.
        item.enabled = false;
        item.transform.SetPositionAndRotation(position, rotation);
        var body = item.GetComponent<Rigidbody>();
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.isKinematic = true;
        body.useGravity = false;
        body.position = position;
        body.rotation = rotation;
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
