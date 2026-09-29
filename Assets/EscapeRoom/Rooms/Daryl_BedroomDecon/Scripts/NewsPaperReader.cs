using UnityEngine;
using UnityEngine.InputSystem;

public class NewsPaperReader : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject newsPaper;
    [SerializeField] private GameObject newsPaperPanel;
    [SerializeField] private GameObject newspaperFullPanel;
    [SerializeField] private InputActionReference readAction;
    [SerializeField] private float readDistance = 3.5f;

    private bool isReading;
    private bool isLookingAtNewspaper;

    private void Start()
    {
        newsPaperPanel.SetActive(false);
        newspaperFullPanel.SetActive(false);
    }
    private void ToggleNewspaper(InputAction.CallbackContext context)
    {
        if (isReading)
        {
            isReading = false;
            newspaperFullPanel.SetActive(false);
        }
        else if (isLookingAtNewspaper)
        {
            isReading = true;
            newspaperFullPanel.SetActive(true);
            newsPaperPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        Canvas newspaperCanvas = GetComponent<Canvas>();
        newspaperCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        newspaperCanvas.worldCamera = playerCamera;
        newspaperCanvas.planeDistance = 0.15f;

        readAction.action.Enable();
        readAction.action.performed += ToggleNewspaper;
    }

    private void OnDisable()
    {
        readAction.action.performed -= ToggleNewspaper;
        readAction.action.Disable();
    }

    private void Update()
    {
        bool hitNewspaper = false;
        RaycastHit[] hits = Physics.RaycastAll(
            playerCamera.transform.position,
            playerCamera.transform.forward,
            readDistance);

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform == newsPaper.transform
                || hit.transform.IsChildOf(newsPaper.transform))
            {
                hitNewspaper = true;
                break;
            }
        }

        isLookingAtNewspaper = hitNewspaper;
        newsPaperPanel.SetActive(isLookingAtNewspaper && !isReading);
    }
}
