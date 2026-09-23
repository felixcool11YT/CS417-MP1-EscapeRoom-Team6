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

    [Header("Keycard")]
    [SerializeField]
    private GameObject KeycardPrefab;

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

        // Spawn the keycard at the dispenser slot
        if (KeycardPrefab != null && KeycardSpawnPoint != null)
        {
            Instantiate(KeycardPrefab, KeycardSpawnPoint.position, KeycardSpawnPoint.rotation);
            Debug.Log("[Puzzle] Keycard spawned.");
        }
        else
        {
            Debug.LogWarning("[Puzzle] Keycard prefab or spawn point not assigned.");
        }
    }
}
