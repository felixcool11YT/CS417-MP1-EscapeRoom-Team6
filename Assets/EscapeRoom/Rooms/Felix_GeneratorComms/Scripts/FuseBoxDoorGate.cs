using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FuseBoxDoorGate : MonoBehaviour
{
    [Header("Door Interaction")]
    public XRSimpleInteractable doorInteractable;

    [Header("Door Bone")]
    public Transform doorBone;

    [Header("Door Audio")]
    public AudioSource doorAudio;

    [Header("Interior Access")]
    public XRSimpleInteractable[] interiorInteractables;

    [Header("Door Rotation")]
    public Vector3 closedEuler;
    public Vector3 openEuler;
    public float rotationSpeed = 4f;

    [Header("State")]
    public bool startLocked = true;

    [Header("Occlusion")]
    public GameObject closedDoorOccluder;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    private void Start()
    {
        if (doorInteractable != null)
        {
            doorInteractable.enabled = !startLocked;
            doorInteractable.selectEntered.AddListener(OnDoorSelected);
        }

        SetInteriorEnabled(false);

        closedRotation = Quaternion.Euler(closedEuler);
        openRotation = Quaternion.Euler(openEuler);

        if (doorBone != null)
            doorBone.localRotation = closedRotation;
    }

    private void SetInteriorEnabled(bool enabled)
    {
        foreach (XRSimpleInteractable interactable in interiorInteractables)
        {
            if (interactable != null)
                interactable.enabled = enabled;
        }
    }

    private void Update()
    {
        if (doorBone == null)
            return;

        Quaternion targetRotation = isOpen ? openRotation : closedRotation;

        doorBone.localRotation = Quaternion.Slerp(
            doorBone.localRotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void OnDestroy()
    {
        if (doorInteractable != null)
            doorInteractable.selectEntered.RemoveListener(OnDoorSelected);
    }

    private void OnDoorSelected(SelectEnterEventArgs args)
    {
        isOpen = !isOpen;

        if (doorAudio != null)
            doorAudio.Play();

        if (closedDoorOccluder != null)
            closedDoorOccluder.SetActive(!isOpen);

        SetInteriorEnabled(isOpen);
    }

    public void UnlockDoor()
    {
        if (doorInteractable != null)
            doorInteractable.enabled = true;

        Debug.Log("Fuse box is now safe and unlocked.");
    }

    public void LockDoor()
    {
        if (doorInteractable != null)
            doorInteractable.enabled = false;
    }
}