using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class KeypadButton : MonoBehaviour
{
    
    [SerializeField] private KeypadLockerController keypad;
    [SerializeField] private string buttonValue;

    private XRSimpleInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
    }

    private void OnEnable()
    {
        interactable.selectEntered.AddListener(Press);
    }

    private void OnDisable()
    {
        interactable.selectEntered.RemoveListener(Press);
    }

    private void Press(SelectEnterEventArgs args)
    {
        if (int.TryParse(buttonValue, out int digit))
        {
            keypad.PressDigit(digit);
        }
        else if (buttonValue == "C")
        {
            keypad.Clear();
        }
        else if (buttonValue == "E")
        {
            keypad.Submit();
        }
    }
}
