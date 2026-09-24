using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GeneratorCoreSocket : MonoBehaviour
{
    [Header("Socket")]
    public Transform snapPoint;

    [Header("Feedback")]
    public AudioSource insertAudio;
    public ParticleSystem insertFX;

    [Header("Completion")]
    public GameObject finalSharedItem;
    public UnityEvent onCoreInserted;

    private bool completed = false;

    public GameObject socketGlow;
    public XRHoverHighlight socketHighlight;


    private void OnTriggerStay(Collider other)
    {
        if (completed)
            return;

        GeneratorCoreItem core =
            other.GetComponentInParent<GeneratorCoreItem>();

        if (core == null)
            return;

        XRGrabInteractable grab =
            core.GetComponent<XRGrabInteractable>();

        if (grab != null && grab.isSelected)
            return;


        InsertCore(core.gameObject);

    }

    private void InsertCore(GameObject core)
    {
        completed = true;

        Rigidbody rb = core.GetComponent<Rigidbody>();
        XRGrabInteractable grab =
            core.GetComponent<XRGrabInteractable>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        if (grab != null)
            grab.enabled = false;

        if (snapPoint != null)
        {
            core.transform.position = snapPoint.position;
            core.transform.rotation = snapPoint.rotation;
        }

        if (insertAudio != null)
            insertAudio.Play();

        if (socketHighlight != null)
            socketHighlight.DisableHighlight();

        if (socketGlow != null)
            socketGlow.SetActive(false);

        if (insertFX != null)
            insertFX.Play();

        if (finalSharedItem != null)
            finalSharedItem.SetActive(true);

        onCoreInserted?.Invoke();

        Debug.Log("Generator core installed. Full power restored.");
    }
}