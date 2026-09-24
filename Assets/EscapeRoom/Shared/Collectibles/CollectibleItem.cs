using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class CollectibleItem : MonoBehaviour
{
    [SerializeField] private string itemName = "Bunker Keepsake";
    [SerializeField] private GameFlowController gameFlow;

    private XRGrabInteractable grabInteractable;
    private bool collected;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (gameFlow == null)
            gameFlow = FindFirstObjectByType<GameFlowController>();
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(Collect);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(Collect);
    }

    private void Collect(SelectEnterEventArgs args)
    {
        if (collected || gameFlow == null)
            return;

        collected = true;
        gameFlow.AddCollectible(itemName);
        Destroy(gameObject);
    }
}
