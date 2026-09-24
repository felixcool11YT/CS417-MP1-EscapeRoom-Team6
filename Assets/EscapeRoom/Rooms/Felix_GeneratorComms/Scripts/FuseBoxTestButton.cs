using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FuseBoxTestButton : MonoBehaviour
{
    [Header("Interaction")]
    public XRSimpleInteractable interactable;

    [Header("Button Bone")]
    public Transform buttonBone;

    [Header("Movement")]
    public Vector3 restPosition;
    public Vector3 pressedPosition;
    public float pressTime = 0.12f;
    public float holdTime = 0.12f;

    [Header("Puzzle")]
    public FuseBoxPuzzle puzzle;

    private bool pressing = false;

    private void Start()
    {
        if (interactable == null)
            interactable = GetComponent<XRSimpleInteractable>();

        if (interactable != null)
            interactable.selectEntered.AddListener(OnPressed);

        if (buttonBone != null)
            buttonBone.localPosition = restPosition;
    }

    private void OnDestroy()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnPressed);
    }

    private void OnPressed(SelectEnterEventArgs args)
    {
        if (!pressing)
            StartCoroutine(PressRoutine());
    }

    private IEnumerator PressRoutine()
    {
        pressing = true;

        yield return MoveButton(restPosition, pressedPosition, pressTime);

        if (puzzle != null)
            puzzle.CheckSolution();

        yield return new WaitForSeconds(holdTime);

        yield return MoveButton(pressedPosition, restPosition, pressTime);

        pressing = false;
    }

    private IEnumerator MoveButton(Vector3 from, Vector3 to, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            if (buttonBone != null)
                buttonBone.localPosition =
                    Vector3.Lerp(from, to, time / duration);

            yield return null;
        }

        if (buttonBone != null)
            buttonBone.localPosition = to;
    }
}