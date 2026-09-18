using UnityEngine;
using UnityEngine.InputSystem;

public class LightColorController : MonoBehaviour
{
    [SerializeField]
    private InputActionReference lightAction;

 
    private Light roomLight;
 
 void Start()
    {
        roomLight = GetComponent<Light>();
    }

 void ChangeLight(InputAction.CallbackContext callback)
    {
        if(roomLight.color == Color.red)
        {
            roomLight.color = Color.white;
        } else
        {
          roomLight.color = Color.red;  
        }
        
    }
    
 void OnEnable()
    {
        lightAction.action.Enable();
        lightAction.action.performed += ChangeLight;
        
    }

    void OnDisable()
    {
        lightAction.action.Disable();
        lightAction.action.performed -= ChangeLight;
    }
}
