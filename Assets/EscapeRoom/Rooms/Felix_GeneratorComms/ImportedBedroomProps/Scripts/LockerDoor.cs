using System.Collections;
using UnityEngine;

public class LockerDoor : MonoBehaviour
{
    public float openAngle = 100f;
    public float duration = 0.5f;

    public AudioSource doorAudio;
    public AudioClip creakSound;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    private bool open = false;
    private bool moving = false;

    void Start()
    {
        closedRotation = transform.localRotation;

        openRotation =
            closedRotation *
            Quaternion.AngleAxis(
                openAngle,
                Vector3.up
            );
    }

    public void ToggleDoor()
    {
        if (moving)
            return;

        open = !open;

        if (doorAudio != null && creakSound != null)
        {
            doorAudio.PlayOneShot(creakSound);
        }

        StartCoroutine(
            RotateDoor(
                open ? openRotation : closedRotation
            )
        );
    }

    private IEnumerator RotateDoor(
        Quaternion targetRotation
    )
    {
        moving = true;

        Quaternion startRotation =
            transform.localRotation;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(elapsed / duration);

            t = t * t * (3f - 2f * t);

            transform.localRotation =
                Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    t
                );

            yield return null;
        }

        transform.localRotation = targetRotation;
        moving = false;
    }
}