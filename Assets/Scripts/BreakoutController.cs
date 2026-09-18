using UnityEngine;
using UnityEngine.InputSystem;


public class BreakoutController : MonoBehaviour
{
    [SerializeField]
    private InputActionReference viewToggle;

    [SerializeField]
    private Transform xrOrigin;

    [SerializeField]
    private Transform insideView;

    [SerializeField]
    private Transform outsideView;

    private bool isOutside = false;

    private void ChangeView(InputAction.CallbackContext callback)
    {
        if(isOutside)
        {
            xrOrigin.position = insideView.position;
            isOutside = false;
        } else
        {
            xrOrigin.position = outsideView.position;
            isOutside = true;
        }
    }

    void OnEnable()
    {
        viewToggle.action.Enable();
        viewToggle.action.performed += ChangeView;
    }

        void OnDisable()
    {
        viewToggle.action.Disable();
        viewToggle.action.performed -= ChangeView;
    }
}
