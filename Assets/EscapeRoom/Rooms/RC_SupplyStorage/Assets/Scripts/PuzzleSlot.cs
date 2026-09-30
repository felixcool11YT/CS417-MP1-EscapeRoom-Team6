using UnityEngine;

public class PuzzleSlot : MonoBehaviour
{
    [SerializeField]
    private SupplyBoxPuzzle puzzle;

    private void OnTriggerEnter(Collider other) => TryPlace(other);
    private void OnTriggerStay(Collider other) => TryPlace(other);

    private void TryPlace(Collider other)
    {
        PuzzleItem item = other.GetComponentInParent<PuzzleItem>();

        if (item == null || puzzle == null)
            return;

        puzzle.TryPlaceItem(item);
    }
}
