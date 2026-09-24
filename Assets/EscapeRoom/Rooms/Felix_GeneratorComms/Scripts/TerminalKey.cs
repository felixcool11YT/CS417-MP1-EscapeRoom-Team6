using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TerminalKey : MonoBehaviour
{
    public TerminalCodePuzzle terminal;

    [Header("Key")]
    public string character;

    public bool isEnter = false;
    public bool isClear = false;

    private XRSimpleInteractable interactable;

    private void Start()
    {
        interactable = GetComponent<XRSimpleInteractable>();

        if (interactable != null)
            interactable.selectEntered.AddListener(OnPressed);
    }

    private void OnDestroy()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnPressed);
    }

    private void OnPressed(SelectEnterEventArgs args)
    {
        if (terminal == null)
            return;

        if (isEnter)
        {
            terminal.SubmitCode();
        }
        else if (isClear)
        {
            terminal.ClearInput();
        }
        else
        {
            terminal.AddCharacter(character);
        }
    }
}