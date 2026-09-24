using System.Collections.Generic;
using UnityEngine;

public class SupplyBoxPuzzle : MonoBehaviour
{
    [Header("Snap Points")]
    [SerializeField] private Transform medicalKitPoint;
    [SerializeField] private Transform walkieTalkiePoint;
    [SerializeField] private Transform pillsPoint;
    [SerializeField] private Transform flareGunPoint;

    [Header("Completion")]
    [SerializeField] private Animator puzzleBoxAnimator;
    [SerializeField] private Animator rewardBoxAnimator;

    [Header("Feedback")]
    [SerializeField] private AudioSource placementSound;
    [SerializeField] private ParticleSystem placementParticles;

    private HashSet<SupplyItemType> placedItems = new();
    private bool puzzleSolved = false;

    public void TryPlaceItem(PuzzleItem item)
    {
        if (puzzleSolved || item == null)
            return;

        // Don't accept the same item type twice.
        if (placedItems.Contains(item.itemType))
            return;

        Transform snapPoint = GetSnapPoint(item.itemType);

        if (snapPoint == null)
            return;

        SnapItem(item.gameObject, snapPoint);

        placedItems.Add(item.itemType);

        PlayPlacementFeedback(snapPoint);

        CheckPuzzle();
    }

    private Transform GetSnapPoint(SupplyItemType itemType)
    {
        switch (itemType)
        {
            case SupplyItemType.MedicalKit:
                return medicalKitPoint;

            case SupplyItemType.WalkieTalkie:
                return walkieTalkiePoint;

            case SupplyItemType.Pills:
                return pillsPoint;

            case SupplyItemType.FlareGun:
                return flareGunPoint;

            default:
                return null;
        }
    }

    private void SnapItem(GameObject item, Transform snapPoint)
    {
        item.transform.position = snapPoint.position;
        item.transform.rotation = snapPoint.rotation;

        Rigidbody rb = item.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Prevent the player from grabbing the item again.
        var grabInteractable =
            item.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        if (grabInteractable != null)
        {
            grabInteractable.enabled = false;
        }
    }

    private void PlayPlacementFeedback(Transform snapPoint)
    {
        if (placementSound != null)
        {
            placementSound.Play();
        }

        if (placementParticles != null)
        {
            placementParticles.transform.position = snapPoint.position;
            placementParticles.Play();
        }
    }

    private void CheckPuzzle()
    {
        if (placedItems.Count < 4)
            return;

        SolvePuzzle();
    }

    private void SolvePuzzle()
    {
        puzzleSolved = true;

        Debug.Log("Supply box puzzle solved!");

        // Close the puzzle box.
        if (puzzleBoxAnimator != null)
        {
            puzzleBoxAnimator.SetTrigger("Close");
        }

        // Open the reward box.
        if (rewardBoxAnimator != null)
        {
            rewardBoxAnimator.SetTrigger("Open");
        }
    }
}