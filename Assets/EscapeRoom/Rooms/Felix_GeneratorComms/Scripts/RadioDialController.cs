using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class RadioDialController : MonoBehaviour
{
    [Header("Interaction")]
    public XRSimpleInteractable interactable;

    [Header("Dial")]
    public Transform dialVisual;
    public float rotationSpeed = 8f;

    [Header("Radio Audio")]
    public AudioSource radioAudio;
    public AudioClip staticClip;
    public AudioClip gibberishClip;
    public AudioClip transmissionClip;
    public AudioClip musicClip;

    [Header("Starting Station")]
    [Range(0, 3)]
    public int startStation = 0;

    private int currentStation;

    public int CurrentStation => currentStation;
    private Quaternion targetRotation;

    private bool powered = false;
    public bool IsPowered => powered;

    // These are the four rotations you found in Unity.
    private readonly Vector3[] stationRotations =
    {
        new Vector3(-89.899f, 0f, 89.44f),
        new Vector3(0.023f, -90.658f, 179.988f),
        new Vector3(108.791f, -90.622f, 180.038f),
        new Vector3(180.757f, -90.658f, 180.012f)
    };

    private void Start()
    {
        if (interactable == null)
            interactable = GetComponent<XRSimpleInteractable>();

        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnDialSelected);
            interactable.enabled = false;
        }

        SetStation(startStation, true);
    }

    private void Update()
    {
        if (dialVisual == null)
            return;

        dialVisual.localRotation = Quaternion.Slerp(
            dialVisual.localRotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void OnDestroy()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnDialSelected);
    }

    private void OnDialSelected(SelectEnterEventArgs args)
    {
        if (!powered)
            return;

        int nextStation = (currentStation + 1) % 4;
        SetStation(nextStation, false);
    }

    private void SetStation(int station, bool instant)
    {
        currentStation = station;

        targetRotation = Quaternion.Euler(stationRotations[currentStation]);

        if (instant && dialVisual != null)
            dialVisual.localRotation = targetRotation;

        PlayStationAudio();
    }

    public void SetPowered(bool value)
    {
        powered = value;

        if (interactable != null)
            interactable.enabled = powered;

        if (radioAudio != null)
        {
            radioAudio.Stop();

            if (powered)
                PlayStationAudio();
        }

    }

    private void PlayStationAudio()
    {
        if (!powered || radioAudio == null)
            return;

        radioAudio.Stop();

        switch (currentStation)
        {
            // Station 1: static
            case 0:
                radioAudio.clip = staticClip;
                radioAudio.loop = true;
                break;

            // Station 2: gibberish
            case 1:
                radioAudio.clip = gibberishClip;
                radioAudio.loop = true;
                break;

            // Station 3: actual CS 417 transmission
            case 2:
                radioAudio.clip = transmissionClip;
                radioAudio.loop = true;
                break;

            // Station 4: spooky/music broadcast
            case 3:
                radioAudio.clip = musicClip;
                radioAudio.loop = true;
                break;
        }

        if (radioAudio.clip != null)
            radioAudio.Play();

        Debug.Log("Radio station: " + (currentStation + 1));
    }
}