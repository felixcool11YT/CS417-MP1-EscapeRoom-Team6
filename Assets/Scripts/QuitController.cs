using UnityEngine;
using UnityEngine.InputSystem;

public class QuitController : MonoBehaviour
{
    [SerializeField]
    private InputActionReference quitAction;

    void OnQuit(InputAction.CallbackContext callback)
    {
        Debug.Log("Quit Requested");

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else 
            Application.Quit();
        #endif
    }

    void OnEnable()
    {
        quitAction.action.Enable();
        quitAction.action.performed += OnQuit;
        
    }

    void OnDisable()
    {
        quitAction.action.Disable();
        quitAction.action.performed -= OnQuit;
    }

}
