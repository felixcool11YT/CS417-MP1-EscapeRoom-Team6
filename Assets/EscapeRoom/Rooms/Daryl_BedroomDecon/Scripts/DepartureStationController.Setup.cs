using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


public partial class DepartureStationController
{
    [Header("Packing")]
    [SerializeField] private XRSocketInteractor[] supplySockets;
    [SerializeField] private XRGrabInteractable[] supplyItems;
    [SerializeField] private TMP_Text packingText;
    [Header("Purge")]
    [SerializeField] private XRSimpleInteractable[] filterControls;
    [SerializeField] private Renderer[] filterLamps;
    [SerializeField] private TMP_Text purgeText;
    [Header("Power")]
    [SerializeField] private BreakerSwitch[] breakers; // FILTERS, HATCH, AUX
    [SerializeField] private XRSimpleInteractable testButton;
    [Header("Station feedback")]
    [SerializeField] private TMP_Text stationText;
    [SerializeField] private Renderer[] stageLamps;
    [SerializeField] private Material neutralMaterial, readyMaterial, completeMaterial;
    [SerializeField] private AudioSource acceptedAudio, rejectedAudio, completedAudio;
    [SerializeField] private FinalDoorController finalDoor;

    private int filterStep;
    private int packedCount;
    private Stage displayedStage = Stage.AwaitingAuthorization;
    private int displayedFilterStep;

    private void OnEnable()
    {
        foreach (var socket in supplySockets)
        {
            socket.selectEntered.AddListener(OnSupplyInserted);
            socket.selectExited.AddListener(OnSupplyRemoved);
        }
    }

    private void OnDisable()
    {
        foreach (var socket in supplySockets)
        {
            socket.selectEntered.RemoveListener(OnSupplyInserted);
            socket.selectExited.RemoveListener(OnSupplyRemoved);
        }
        CancelInvoke();
    }

    private void Start() => RefreshStage();
    private void OnSupplyInserted(SelectEnterEventArgs args) => UpdatePacking();
    private void OnSupplyRemoved(SelectExitEventArgs args) => UpdatePacking();

    private void UpdatePacking()
    {
        if (currentStage != Stage.Packing) return;
        packedCount = 0;
        foreach (var socket in supplySockets)
            if (socket.hasSelection) packedCount++;
        if (packedCount == supplySockets.Length)
        {
            // Latch the completed step before release callbacks fire. Stored
            // supplies stay in the box and cannot be removed after packing.
            currentStage = Stage.Purging;
            for (int i = 0; i < supplyItems.Length; i++)
            {
                var item = supplyItems[i];
                var target = supplySockets[i].attachTransform;
                // Selection fires before the socket's smoothing finishes. Seat
                // the item at its anchor before freezing its final pose.
                item.transform.rotation = target.rotation
                    * Quaternion.Inverse(item.attachTransform.rotation) * item.transform.rotation;
                item.transform.position += target.position - item.attachTransform.position;
                var position = item.transform.position;
                var rotation = item.transform.rotation;
                item.enabled = false;
                item.transform.SetPositionAndRotation(position, rotation);
                var body = item.GetComponent<Rigidbody>();
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                body.isKinematic = true;
                body.useGravity = false;
                body.position = position;
                body.rotation = rotation;
            }
        }
        RefreshStage();
    }

    public void CheckPower()
    {
        if (currentStage != Stage.Powering) return;
        if (!breakers[0].isOn || !breakers[1].isOn || breakers[2].isOn)
        {
            RejectInput();
            return;
        }
        currentStage = Stage.Complete;
        RefreshStage();
        if (completedAudio != null) completedAudio.Play();
        finalDoor.CompleteDeparture();
    }

    private void RejectInput()
    {
        RefreshStage();
        if (rejectedAudio != null) rejectedAudio.Play();
        stationText.text = currentStage == Stage.Purging
            ? "WRONG FILTER ORDER / START AGAIN"
            : "CHECK THE BREAKERS / READ R'S NOTE";
        Invoke(nameof(RefreshStage), 1.5f);
    }

    private void RefreshStage()
    {
        CancelInvoke(nameof(RefreshStage));
        bool packing = currentStage == Stage.Packing;
        bool purging = currentStage == Stage.Purging;
        bool powering = currentStage == Stage.Powering;
        bool complete = currentStage == Stage.Complete;
        foreach (var socket in supplySockets) socket.socketActive = packing;
        foreach (var item in supplyItems) item.enabled = packing;
        foreach (var control in filterControls) control.enabled = purging;
        foreach (var breaker in breakers)
            breaker.GetComponent<XRSimpleInteractable>().enabled = powering;
        testButton.enabled = powering;

        packingText.text = $"PACKED {packedCount} / 2";
        purgeText.text = $"CYCLES {filterStep} / 3";
        for (int i = 0; i < filterLamps.Length; i++)
            filterLamps[i].sharedMaterial = i < filterStep ? completeMaterial : neutralMaterial;
        int activeStage = (int)currentStage - 1;
        for (int i = 0; i < stageLamps.Length; i++)
            stageLamps[i].sharedMaterial = i < activeStage ? completeMaterial
                : i == activeStage ? readyMaterial : neutralMaterial;

        switch (currentStage)
        {
            case Stage.AwaitingAuthorization: stationText.text = "INSERT 3 CLEARANCES AT THE DOOR"; break;
            case Stage.Packing: stationText.text = "1 / PACK THE EXIT SUPPLIES"; break;
            case Stage.Purging: stationText.text = "2 / RECALL THE FILTER ORDER"; break;
            case Stage.Powering: stationText.text = "3 / SET BREAKERS, THEN PRESS TEST"; break;
            case Stage.Complete: stationText.text = "HATCH RELEASED / PROCEED UPSTAIRS"; break;
        }
        if (!complete && (currentStage != displayedStage || filterStep > displayedFilterStep))
            if (acceptedAudio != null) acceptedAudio.Play();
        displayedStage = currentStage;
        displayedFilterStep = filterStep;
    }
}
