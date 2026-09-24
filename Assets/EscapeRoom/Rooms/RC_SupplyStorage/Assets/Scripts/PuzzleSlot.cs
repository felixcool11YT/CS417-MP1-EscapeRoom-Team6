using UnityEngine;

public class PuzzleSlot : MonoBehaviour
{
    [SerializeField]
    private SupplyBoxPuzzle puzzle;

    private void OnTriggerEnter(Collider other)
    {
        PuzzleItem item = other.GetComponentInParent<PuzzleItem>();

        if (item == null)
            return;

        puzzle.TryPlaceItem(item);
    }
}