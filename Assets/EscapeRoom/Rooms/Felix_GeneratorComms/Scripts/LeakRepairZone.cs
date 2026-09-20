using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class LeakRepairZone : MonoBehaviour
{
    public enum LeakType
    {
        LeakA,
        LeakB
    }

    [Header("Leak")]
    public WaterLeakPuzzle waterLeakPuzzle;
    public LeakType leakType;

    [Header("Repair")]
    public float repairTime = 1.0f;

    private float currentRepairTime = 0f;
    private bool repaired = false;

    private void OnTriggerStay(Collider other)
    {
        if (repaired)
            return;

        SealantTool sealant =
            other.GetComponentInParent<SealantTool>();

        if (sealant == null)
            return;

        XRGrabInteractable grab =
            sealant.GetComponent<XRGrabInteractable>();

        if (grab == null || !grab.isSelected)
        {
            currentRepairTime = 0f;
            return;
        }

        currentRepairTime += Time.deltaTime;

        if (currentRepairTime >= repairTime)
        {
            RepairLeak();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        SealantTool sealant =
            other.GetComponentInParent<SealantTool>();

        if (sealant != null && !repaired)
            currentRepairTime = 0f;
    }

    private void RepairLeak()
    {
        repaired = true;

        if (waterLeakPuzzle == null)
            return;

        if (leakType == LeakType.LeakA)
            waterLeakPuzzle.SealLeakA();
        else
            waterLeakPuzzle.SealLeakB();

        gameObject.SetActive(false);
    }
}