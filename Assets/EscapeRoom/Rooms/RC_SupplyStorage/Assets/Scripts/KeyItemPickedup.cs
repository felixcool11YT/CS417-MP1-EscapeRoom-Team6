using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class KeyItemFeedback : MonoBehaviour
{
    [SerializeField] private AudioSource pickupSound;
    [SerializeField] private ParticleSystem pickupParticles;

    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.AddListener(OnPickedUp);
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.RemoveListener(OnPickedUp);
    }

    private void OnPickedUp(SelectEnterEventArgs args)
    {
        if (pickupSound != null)
            pickupSound.Play();

        if (pickupParticles != null)
            pickupParticles.Play();
    }
}