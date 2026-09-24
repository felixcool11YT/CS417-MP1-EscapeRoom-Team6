using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BunkerHatchController : MonoBehaviour
{
    [Header("Interaction")]
    public XRSimpleInteractable interactable;

    public GameObject hatchPushGlow;

    [Header("Hatch")]
    public Transform hatchPivot;
    public Vector3 closedEuler;
    public Vector3 openEuler;

    [Header("Push Mechanic")]
    public int pushesRequired = 5;
    public float rotationSpeed = 3f;

    [Header("Audio")]
    public AudioSource pushAudio;
    public AudioSource fullyOpenAudio;

    private int pushes = 0;
    private float targetProgress = 0f;
    private bool fullyOpened = false;

    private void Start()
    {
        if (interactable != null)
            interactable.selectEntered.AddListener(OnPush);

        if (hatchPivot != null)
            hatchPivot.localRotation = Quaternion.Euler(closedEuler);
    }

    private void Update()
    {
        if (hatchPivot == null)
            return;

        Quaternion targetRotation = Quaternion.Lerp(
            Quaternion.Euler(closedEuler),
            Quaternion.Euler(openEuler),
            targetProgress
        );

        hatchPivot.localRotation = Quaternion.Slerp(
            hatchPivot.localRotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void OnPush(SelectEnterEventArgs args)
    {
        if (fullyOpened)
            return;

        pushes++;

        if (pushAudio != null)
            pushAudio.Play();

        targetProgress = Mathf.Clamp01(
            (float)pushes / pushesRequired
        );

        if (pushes >= pushesRequired)
        {


            fullyOpened = true;

            if (hatchPushGlow != null)
                hatchPushGlow.SetActive(false);

            if (fullyOpenAudio != null)
                fullyOpenAudio.Play();

            Debug.Log("BUNKER HATCH FULLY OPEN");
        }
    }

    private void OnDestroy()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnPush);
    }
}