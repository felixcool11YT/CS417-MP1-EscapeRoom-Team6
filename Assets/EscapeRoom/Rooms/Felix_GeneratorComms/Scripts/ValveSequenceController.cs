using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ValveSequenceController : MonoBehaviour
{
    [Header("Correct order")]
    public ValvePuzzleValve[] valveOrder;

    [Header("Feedback")]
    public float wrongFlashTime = 0.7f;

    [Header("Puzzle Complete")]
    public UnityEvent onPuzzleComplete;

    private int currentStep = 0;
    private bool resetting = false;
    private bool completed = false;

    public void ValveTurned(ValvePuzzleValve valve)
    {
        if (resetting || completed)
            return;

        // Correct valve
        if (valve == valveOrder[currentStep])
        {
            valve.ShowCorrect();

            currentStep++;

            if (currentStep >= valveOrder.Length)
            {
                completed = true;
                Debug.Log("Valve puzzle complete!");

                onPuzzleComplete.Invoke();
            }
        }
        // Wrong valve
        else
        {
            StartCoroutine(WrongSequence(valve));
        }
    }

    private IEnumerator WrongSequence(ValvePuzzleValve wrongValve)
    {
        resetting = true;

        wrongValve.ShowWrong();

        yield return new WaitForSeconds(wrongFlashTime);

        ResetPuzzle();
        resetting = false;
    }

    private void ResetPuzzle()
    {
        currentStep = 0;

        foreach (ValvePuzzleValve valve in valveOrder)
        {
            valve.ResetValve();
        }
    }
}