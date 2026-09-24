using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FlashlightToggle : MonoBehaviour
{
    [Header("Interaction")]
    public XRGrabInteractable grabInteractable;

    [Header("Flashlight")]
    public Light flashlight;

    [Header("Settings")]
    public bool startsOn = false;

    private bool isOn;

    private void Start()
    {
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        isOn = startsOn;

        if (flashlight != null)
            flashlight.enabled = isOn;

        if (grabInteractable != null)
            grabInteractable.activated.AddListener(OnActivated);
    }

    private void OnActivated(ActivateEventArgs args)
    {
        ToggleLight();
    }

    private void ToggleLight()
    {
        isOn = !isOn;

        if (flashlight != null)
            flashlight.enabled = isOn;
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
            grabInteractable.activated.RemoveListener(OnActivated);
    }
}
