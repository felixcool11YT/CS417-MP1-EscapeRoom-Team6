using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BreakerSwitch : MonoBehaviour
{
    [Header("Breaker Mesh")]
    public Transform breakerVisual;

    [Header("State")]
    public bool startsOn = true;
    public bool isOn;

    [Header("Rotations")]
    public Vector3 onEuler = new Vector3(107.231f, 0f, 0f);
    public Vector3 offEuler = new Vector3(241.885f, 0f, 0f);

    [Header("Puzzle")]
    public FuseBoxPuzzle controller;

    [Header("Audio")]
    public AudioSource breakerAudio;

    private XRSimpleInteractable interactable;

    void Start()
    {
        interactable = GetComponent<XRSimpleInteractable>();

        if (interactable != null)
            interactable.selectEntered.AddListener(OnSelected);

        if (breakerAudio == null)
            breakerAudio = GetComponent<AudioSource>();

        isOn = startsOn;
        UpdateVisual();
    }



    private void OnDestroy()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnSelected);
    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        Toggle();
    }

    public void Toggle()
    {
        isOn = !isOn;
        UpdateVisual();

        if (breakerAudio != null)
            breakerAudio.Play();
    }
    public void SetState(bool value)
    {
        isOn = value;
        UpdateVisual();
    }
    private void UpdateVisual()
    {
        if (breakerVisual == null)
            return;

        breakerVisual.localEulerAngles = isOn ? onEuler : offEuler;
    }
}