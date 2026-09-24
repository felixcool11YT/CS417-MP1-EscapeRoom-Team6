using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class KeypadLockerController : MonoBehaviour
{
    [Header("Keypad")]
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private string correctCode = "2019";

    [Header("Locker")]
    [SerializeField] private Transform lockerDoorHinge;
    [SerializeField] private float openAngle = -105f;
    [SerializeField] private float openDuration = 0.75f;

    [Header("Locked Item")]
    [SerializeField] private XRGrabInteractable lockedFilter;
    [SerializeField] private Rigidbody lockedFilterBody;

    private string currentEntry = "";
    private Quaternion closedDoorRotation;
    private bool unlocked;

    private void Awake()
    {
        closedDoorRotation = lockerDoorHinge.localRotation;
        SetFilterLocked(true);
        UpdateDisplay();
    }

    public void PressDigit(int digit)
    {
        if (unlocked || currentEntry.Length >= correctCode.Length)
        {
            return;
        }

        currentEntry += digit.ToString();
        UpdateDisplay();
    }

    public void Clear()
    {
        if (unlocked)
        {
            return;
        }

        currentEntry = "";
        UpdateDisplay();
    }

    public void Submit()
    {
        if (unlocked)
        {
            return;
        }

        if (currentEntry == correctCode)
        {
            unlocked = true;
            displayText.text = "OPEN";
            SetFilterLocked(false);
            StartCoroutine(OpenDoor());
        }
        else
        {
            currentEntry = "";
            displayText.text = "ERROR";
        }
    }

    private void SetFilterLocked(bool isLocked)
    {
        lockedFilter.enabled = !isLocked;
        lockedFilterBody.isKinematic = isLocked;
        lockedFilterBody.useGravity = !isLocked;
    }

    private void UpdateDisplay()
    {
        displayText.text = currentEntry.Length == 0 ? "----" : currentEntry;
    }

    private IEnumerator OpenDoor()
    {
        Quaternion openRotation = closedDoorRotation * Quaternion.Euler(0f, openAngle, 0f);
        float elapsedTime = 0f;

        while (elapsedTime < openDuration)
        {
            elapsedTime += Time.deltaTime;
            float amount = Mathf.Clamp01(elapsedTime / openDuration);
            lockerDoorHinge.localRotation = Quaternion.Slerp(closedDoorRotation, openRotation, amount);
            yield return null;
        }

        lockerDoorHinge.localRotation = openRotation;
    }
}
