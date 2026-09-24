using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class XRHoverHighlight : MonoBehaviour
{
    [Header("Interaction")]
    public XRBaseInteractable interactable;

    [Header("Visuals")]
    public Renderer[] renderers;

    [Header("Highlight")]
    public Color hoverColor = new Color(0.1f, 1f, 1f, 1f);

    [Range(0f, 1f)]
    public float highlightStrength = 0.8f;

    public float emissionIntensity = 2f;

    [Header("Optional Proxy")]
    public GameObject highlightProxy;

    private Material[][] materials;

    private Color[][] colorsBeforeHover;
    private Color[][] emissionsBeforeHover;
    private bool[][] emissionWasEnabled;

    private bool isHighlighted = false;
    private bool suppressHighlight = false;
    private bool highlightEnabled = true;

    private void Awake()
    {
        if (interactable == null)
            interactable = GetComponent<XRBaseInteractable>();

        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>(true);

        if (highlightProxy != null)
            highlightProxy.SetActive(false);

        materials = new Material[renderers.Length][];
        colorsBeforeHover = new Color[renderers.Length][];
        emissionsBeforeHover = new Color[renderers.Length][];
        emissionWasEnabled = new bool[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            materials[i] = renderers[i].materials;

            colorsBeforeHover[i] = new Color[materials[i].Length];
            emissionsBeforeHover[i] = new Color[materials[i].Length];
            emissionWasEnabled[i] = new bool[materials[i].Length];
        }

        if (interactable != null)
        {
            interactable.hoverEntered.AddListener(OnHoverEntered);
            interactable.hoverExited.AddListener(OnHoverExited);

            interactable.selectEntered.AddListener(OnSelected);
            interactable.selectExited.AddListener(OnSelectExited);
        }
        else
        {
            Debug.LogWarning(name + ": XRHoverHighlight has no interactable.");
        }
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (!highlightEnabled)
            return;

        if (suppressHighlight)
            return;

        if (interactable != null && interactable.isSelected)
            return;

        ApplyHighlight();
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        RestoreHighlight();

        suppressHighlight = false;
    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        suppressHighlight = true;
        RestoreHighlight();
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        RestoreHighlight();

        suppressHighlight = true;
    }

    private void ApplyHighlight()
    {
        if (isHighlighted)
            return;

        isHighlighted = true;

        for (int i = 0; i < materials.Length; i++)
        {
            for (int j = 0; j < materials[i].Length; j++)
            {
                Material mat = materials[i][j];

                if (mat.HasProperty("_BaseColor"))
                {
                    Color original = mat.GetColor("_BaseColor");
                    colorsBeforeHover[i][j] = original;

                    mat.SetColor(
                        "_BaseColor",
                        Color.Lerp(original, hoverColor, highlightStrength)
                    );
                }
                else if (mat.HasProperty("_Color"))
                {
                    Color original = mat.GetColor("_Color");
                    colorsBeforeHover[i][j] = original;

                    mat.SetColor(
                        "_Color",
                        Color.Lerp(original, hoverColor, highlightStrength)
                    );
                }

                if (mat.HasProperty("_EmissionColor"))
                {
                    emissionsBeforeHover[i][j] =
                        mat.GetColor("_EmissionColor");

                    emissionWasEnabled[i][j] =
                        mat.IsKeywordEnabled("_EMISSION");

                    mat.EnableKeyword("_EMISSION");

                    mat.SetColor(
                        "_EmissionColor",
                        hoverColor * emissionIntensity
                    );
                }
            }
        }

        if (highlightProxy != null)
            highlightProxy.SetActive(true);
    }

    private void RestoreHighlight()
    {
        if (!isHighlighted)
        {
            if (highlightProxy != null)
                highlightProxy.SetActive(false);

            return;
        }

        for (int i = 0; i < materials.Length; i++)
        {
            for (int j = 0; j < materials[i].Length; j++)
            {
                Material mat = materials[i][j];

                if (mat.HasProperty("_BaseColor"))
                    mat.SetColor("_BaseColor", colorsBeforeHover[i][j]);
                else if (mat.HasProperty("_Color"))
                    mat.SetColor("_Color", colorsBeforeHover[i][j]);

                if (mat.HasProperty("_EmissionColor"))
                {
                    mat.SetColor(
                        "_EmissionColor",
                        emissionsBeforeHover[i][j]
                    );

                    if (!emissionWasEnabled[i][j])
                        mat.DisableKeyword("_EMISSION");
                }
            }
        }

        if (highlightProxy != null)
            highlightProxy.SetActive(false);

        isHighlighted = false;
    }

    public void DisableHighlight()
    {
        highlightEnabled = false;
        suppressHighlight = true;
        RestoreHighlight();

        if (highlightProxy != null)
            highlightProxy.SetActive(false);
    }

    public void EnableHighlight()
    {
        highlightEnabled = true;
        suppressHighlight = false;
    }

    private void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(OnHoverEntered);
            interactable.hoverExited.RemoveListener(OnHoverExited);

            interactable.selectEntered.RemoveListener(OnSelected);
            interactable.selectExited.RemoveListener(OnSelectExited);
        }
    }
}