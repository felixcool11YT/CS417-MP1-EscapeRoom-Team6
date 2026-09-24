using UnityEngine;

public class WallCabinetDoor : MonoBehaviour
{
    [Header("Door")]
    public Transform door;

    [Header("Rotation")]
    public Vector3 closedEuler = new Vector3(-89.98f, 0f, 0f);
    public Vector3 openEuler = new Vector3(-90f, 0f, 106.48f);
    public float rotationSpeed = 4f;

    [Header("Audio")]
    public AudioSource unlockAudio;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    private void Start()
    {
        closedRotation = Quaternion.Euler(closedEuler);
        openRotation = Quaternion.Euler(openEuler);

        if (door != null)
            door.localRotation = closedRotation;
    }

    private void Update()
    {
        if (door == null)
            return;

        Quaternion target = isOpen ? openRotation : closedRotation;

        door.localRotation = Quaternion.Slerp(
            door.localRotation,
            target,
            rotationSpeed * Time.deltaTime
        );
    }

    public void OpenDoor()
    {
        if (isOpen)
            return;

        isOpen = true;

        if (unlockAudio != null)
            unlockAudio.Play();

        Debug.Log("Wall cabinet opened.");
    }
}