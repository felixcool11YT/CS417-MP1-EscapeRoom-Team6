using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody), typeof(XRGrabInteractable))]
public class DroppedItemRecovery : MonoBehaviour
{
    [SerializeField] private Transform recoveryPoint;
    [SerializeField] private float minimumHeight = -1.2f;

    private Rigidbody body;
    private XRGrabInteractable grab;
    private Vector3 startingPosition;
    private Quaternion startingRotation;
    private bool returning;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();
        startingPosition = transform.position;
        startingRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        // Disabled grabs are locked contents or permanently installed items.
        if (returning || !grab.enabled || body.position.y >= minimumHeight)
            return;

        StartCoroutine(ReturnItem());
    }

    private IEnumerator ReturnItem()
    {
        returning = true;
        bool couldThrow = grab.throwOnDetach;
        grab.throwOnDetach = false;
        if (grab.isSelected)
            grab.interactionManager.CancelInteractableSelection((IXRSelectInteractable)grab);

        var position = recoveryPoint != null ? recoveryPoint.position : startingPosition;
        var rotation = recoveryPoint != null ? recoveryPoint.rotation : startingRotation;
        if (!body.isKinematic)
        {
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }
        body.position = position;
        body.rotation = rotation;
        transform.SetPositionAndRotation(position, rotation);

        // XR finishes a release in LateUpdate; don't apply its old throw velocity
        // at the recovery point when a held item was pulled out of the level.
        yield return null;
        grab.throwOnDetach = couldThrow;
        returning = false;
    }
}
