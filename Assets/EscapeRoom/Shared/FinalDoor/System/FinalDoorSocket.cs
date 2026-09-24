using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRSocketInteractor))]
public class FinalDoorSocket : MonoBehaviour
{
    [SerializeField] private FinalDoorController doorController;

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

        XRGrabInteractable insertedItem =
            args.interactableObject.transform.GetComponent<XRGrabInteractable>();

        if (insertedItem != null)
            insertedItem.enabled = false;
    }
}
