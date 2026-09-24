using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Collections;
using UnityEngine.Events;
public class DecontaminationPuzzleController : MonoBehaviour
{
    private int currentStep = 0;
    private bool solved = false;
    private Coroutine blinkRoutine;
    [SerializeField] private PuzzleProgressDisplay progressDisplay;

    [Header("Sockets (in order)")]
    [SerializeField]
    private XRSocketInteractor SocketParticulate;


    [SerializeField]
    private Material StatusBlinkMaterial;

    [SerializeField]
    private Material StatusDarkMaterial;

    [SerializeField]
    private float blinkInterval = 0.5f; 

    [SerializeField]
    private XRSocketInteractor SocketChemical;

    [SerializeField]
    private XRSocketInteractor SocketRadiation;

    [Header("Dispenser")]
    [SerializeField]
    private MeshRenderer DispenserStatusLight;

    [SerializeField]
    private Material StatusGreenMaterial;

    [Header("Keycard (built at runtime, no prefab needed)")]
    [SerializeField]
    private Mesh KeycardMesh;

    [SerializeField]
    private Material[] KeycardMaterials;

    [SerializeField]
    private Transform KeycardSpawnPoint;

    [Header("Room Completion")]
    [SerializeField]
    private UnityEvent OnPuzzleSolved;
        private IEnumerator BlinkStatusLight()
    {
        while(!solved)
        {
            DispenserStatusLight.material = StatusBlinkMaterial;
            yield return new WaitForSeconds(blinkInterval);
            DispenserStatusLight.material = StatusDarkMaterial;
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    void Start()
    {
        progressDisplay.ShowProgress(0);
        blinkRoutine = StartCoroutine(BlinkStatusLight());

    }
    private void OnEnable()
    {
        SocketParticulate.selectEntered.AddListener(Solve);
        SocketChemical.selectEntered.AddListener(Solve);
        SocketRadiation.selectEntered.AddListener(Solve);
    }

    private void OnDisable()
    {
        SocketParticulate.selectEntered.RemoveListener(Solve);
        SocketChemical.selectEntered.RemoveListener(Solve);
        SocketRadiation.selectEntered.RemoveListener(Solve);
    }


    private void Solve(SelectEnterEventArgs args)
    {
        if (solved) return;

        if (currentStep == 0 && args.interactorObject is XRSocketInteractor socketP && socketP == SocketParticulate)
        {
            currentStep++;
            progressDisplay.ShowProgress(currentStep);
        }
        else if (currentStep == 1 && args.interactorObject is XRSocketInteractor socketC && socketC == SocketChemical)
        {
            currentStep++;
            progressDisplay.ShowProgress(currentStep);
        }
        else if (currentStep == 2 && args.interactorObject is XRSocketInteractor socketR && socketR == SocketRadiation)
        {
            currentStep++;
            solved = true;
            Solved();
        }
        else
        {
            currentStep = 0;
            progressDisplay.ShowWrongOrder();
        }
    }

    private void Solved()
    {
        Debug.Log("Puzzle solved! Dispensing keycard.");
        progressDisplay.ShowComplete();

        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            blinkRoutine = null;
        }

        // Turn the dispenser status light green
        if (DispenserStatusLight != null && StatusGreenMaterial != null)
        {
            DispenserStatusLight.material = StatusGreenMaterial;
        }
        else
        {
            Debug.LogWarning("[Puzzle] Dispenser status light or green material not assigned.");
        }

        // Build and spawn the keycard
        SpawnKeycard();

        // Lets the integrated game react without knowing how this puzzle works.
        OnPuzzleSolved?.Invoke();
    }

    private void SpawnKeycard()
    {
        if (KeycardMesh == null)
        {
            Debug.LogWarning("[Puzzle] Keycard mesh not assigned. Drag PROP_Keycard model here.");
            return;
        }
        if (KeycardSpawnPoint == null)
        {
            Debug.LogWarning("[Puzzle] Keycard spawn point not assigned.");
            return;
        }

        // Create the keycard GameObject because prefabs in my UI were getting annoying
        GameObject keycard = new GameObject("Keycard");
        keycard.transform.position = KeycardSpawnPoint.position;
        keycard.transform.rotation = KeycardSpawnPoint.rotation;
        keycard.transform.localScale = Vector3.one * 2f;

        // add mesh
        var filter = keycard.AddComponent<MeshFilter>();
        filter.mesh = KeycardMesh;

        var renderer = keycard.AddComponent<MeshRenderer>();
        if (KeycardMaterials != null && KeycardMaterials.Length > 0)
        {
            renderer.materials = KeycardMaterials;
        }

        // Add physics before the grab component so XR can discover everything cleanly.
        var collider = keycard.AddComponent<BoxCollider>();
        collider.center = KeycardMesh.bounds.center;
        collider.size = KeycardMesh.bounds.size;

        var body = keycard.AddComponent<Rigidbody>();
        body.mass = 0.2f;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        keycard.AddComponent<XRGrabInteractable>();

        var rewardLight = keycard.AddComponent<Light>();
        rewardLight.type = LightType.Point;
        rewardLight.color = new Color(0.2f, 1f, 0.45f);
        rewardLight.intensity = 1.5f;
        rewardLight.range = 1.25f;

        Debug.Log("[Puzzle] Keycard spawned and ready for pickup.");
    }
}
