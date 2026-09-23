using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Collections;
public class DecontaminationPuzzleController : MonoBehaviour
{
    private int currentStep = 0;
    private bool solved = false;

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

    private void Start()
    {

        StartCoroutine(BlinkStatusLight());

    }

    private void Solve(SelectEnterEventArgs args)
    {
        if (solved) return;

        if (currentStep == 0 && args.interactorObject is XRSocketInteractor socketP && socketP == SocketParticulate)
        {
            currentStep++;
        }
        else if (currentStep == 1 && args.interactorObject is XRSocketInteractor socketC && socketC == SocketChemical)
        {
            currentStep++;
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
        }
    }
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

    private void Solved()
    {
        Debug.Log("Puzzle solved! Dispensing keycard.");

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

        // add mesh
        var filter = keycard.AddComponent<MeshFilter>();
        filter.mesh = KeycardMesh;

        var renderer = keycard.AddComponent<MeshRenderer>();
        if (KeycardMaterials != null && KeycardMaterials.Length > 0)
        {
            renderer.materials = KeycardMaterials;
        }

        // make grabbable
        var grab = keycard.AddComponent<XRGrabInteractable>();

        // Add a collider     
        var collider = keycard.AddComponent<BoxCollider>();
        // Size the collider to  mesh bounds
        collider.center = KeycardMesh.bounds.center;
        collider.size = KeycardMesh.bounds.size;

        Debug.Log("[Puzzle] Keycard spawned and ready for pickup.");
    }
}
