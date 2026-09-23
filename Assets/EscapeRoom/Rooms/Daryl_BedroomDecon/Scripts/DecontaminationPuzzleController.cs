using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// Controls the decon zone filter puzzle.
/// Sockets must be filled in order: Particulate (1) -> Chemical (2) -> Radiation (3).
/// On solve, the dispenser turns green and spawns the keycard.
/// </summary>
public class DecontaminationPuzzleController : MonoBehaviour
{
    private int currentStep = 0;
    private bool solved = false;

    [Header("Sockets (in order)")]
    [SerializeField]
    private XRSocketInteractor SocketParticulate;

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

        // Create the keycard GameObject
        GameObject keycard = new GameObject("Keycard");
        keycard.transform.position = KeycardSpawnPoint.position;
        keycard.transform.rotation = KeycardSpawnPoint.rotation;

        // Add mesh
        var filter = keycard.AddComponent<MeshFilter>();
        filter.mesh = KeycardMesh;

        var renderer = keycard.AddComponent<MeshRenderer>();
        if (KeycardMaterials != null && KeycardMaterials.Length > 0)
        {
            renderer.materials = KeycardMaterials;
        }

        // Make it grabbable
        var grab = keycard.AddComponent<XRGrabInteractable>();

        // Add a collider so it can be grabbed
        var collider = keycard.AddComponent<BoxCollider>();
        // Size the collider to the mesh bounds
        collider.center = KeycardMesh.bounds.center;
        collider.size = KeycardMesh.bounds.size;

        Debug.Log("[Puzzle] Keycard spawned and ready for pickup.");
    }
}
