using UnityEngine;
using UnityEngine.InputSystem;

public class NewsPaperReader : MonoBehaviour
{
    [System.Serializable]
    private class ReadableNote
    {
        public GameObject target;
        public GameObject prompt;
        public GameObject fullPanel;
    }

    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject newsPaper;
    [SerializeField] private GameObject newsPaperPanel;
    [SerializeField] private GameObject newspaperFullPanel;
    [SerializeField] private InputActionReference readAction;
    [SerializeField] private float readDistance = 3.5f;
    [SerializeField] private ReadableNote[] additionalNotes = new ReadableNote[0];

    private GameObject lookedAtPanel;
    private GameObject openPanel;

    private void Start()
    {
        newsPaperPanel.SetActive(false);
        newspaperFullPanel.SetActive(false);
        foreach (var note in additionalNotes)
        {
            note.prompt.SetActive(false);
            note.fullPanel.SetActive(false);
        }
    }

    private void ToggleNewspaper(InputAction.CallbackContext context)
    {
        if (openPanel != null)
        {
            openPanel.SetActive(false);
            openPanel = null;
        }
        else if (lookedAtPanel != null)
        {
            OpenPanel(lookedAtPanel);
        }
    }

    // Ray selection of a physical note opens the same camera panel as X.
    public void ReadNote(int index)
    {
        if (index < 0 || index >= additionalNotes.Length) return;
        var note = additionalNotes[index];
        if (Vector3.Distance(playerCamera.transform.position, note.target.transform.position) <= readDistance)
            OpenPanel(note.fullPanel);
    }

    private void OpenPanel(GameObject panel)
    {
        if (openPanel != null) openPanel.SetActive(false);
        openPanel = panel;
        openPanel.SetActive(true);
        HidePrompts();
    }

    private void HidePrompts()
    {
        newsPaperPanel.SetActive(false);
        foreach (var note in additionalNotes) note.prompt.SetActive(false);
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
        HidePrompts();
        lookedAtPanel = null;
        if (openPanel != null) return;

        GameObject prompt = null;
        float nearest = float.PositiveInfinity;
        var hits = Physics.RaycastAll(playerCamera.transform.position,
            playerCamera.transform.forward, readDistance);
        foreach (var hit in hits)
        {
            if (hit.distance >= nearest) continue;
            if (Matches(hit.transform, newsPaper))
            {
                nearest = hit.distance;
                prompt = newsPaperPanel;
                lookedAtPanel = newspaperFullPanel;
            }
            foreach (var note in additionalNotes)
            {
                if (hit.distance < nearest && Matches(hit.transform, note.target))
                {
                    nearest = hit.distance;
                    prompt = note.prompt;
                    lookedAtPanel = note.fullPanel;
                }
            }
        }
        if (prompt != null) prompt.SetActive(true);
    }

    private static bool Matches(Transform hit, GameObject target)
    {
        return target != null && (hit == target.transform || hit.IsChildOf(target.transform));
    }
}
