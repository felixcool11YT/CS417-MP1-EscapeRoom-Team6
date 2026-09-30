using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody), typeof(XRGrabInteractable))]
public class DroppedItemRecovery : MonoBehaviour
{
    [SerializeField] private Transform recoveryPoint;
    [SerializeField] private float minimumHeight = -1.2f;

    private Rigidbody body;
    private XRGrabInteractable grab;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();
    }

    private void FixedUpdate()
    {
        if (recoveryPoint == null || body.isKinematic || grab.isSelected
            || body.position.y >= minimumHeight)
            return;

        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.position = recoveryPoint.position;
        body.rotation = recoveryPoint.rotation;
        transform.SetPositionAndRotation(body.position, body.rotation);
    }
}
