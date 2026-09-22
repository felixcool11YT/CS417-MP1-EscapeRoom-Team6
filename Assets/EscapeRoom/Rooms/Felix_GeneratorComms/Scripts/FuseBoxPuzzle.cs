using UnityEngine;
using UnityEngine.Events;

public class FuseBoxPuzzle : MonoBehaviour
{
    [System.Serializable]
    public class BreakerTarget
    {
        public BreakerSwitch breaker;
        public bool shouldBeOn;
    }

    [Header("Required Configuration")]
    public BreakerTarget[] targets;

    [Header("All Breakers - used for danger reset")]
    public BreakerSwitch[] allBreakers;

    [Header("Feedback")]
    public AudioSource correctAudio;
    public AudioSource wrongAudio;

    [Header("Success")]
    public UnityEvent onPuzzleSolved;

    private bool solved = false;

    public void CheckSolution()
    {
        if (solved)
            return;

        foreach (BreakerTarget target in targets)
        {
            if (target.breaker == null)
                continue;

            if (target.breaker.isOn != target.shouldBeOn)
            {
                WrongSolution();
                return;
            }
        }

        SolvePuzzle();
    }

    private void SolvePuzzle()
    {
        solved = true;

        if (correctAudio != null)
            correctAudio.Play();

        onPuzzleSolved?.Invoke();

        Debug.Log("FUSE BOX SOLVED!");
    }

    private void WrongSolution()
    {
        if (wrongAudio != null)
            wrongAudio.Play();

        Debug.Log("Incorrect fuse box configuration.");
    }

    public void ResetAllBreakers()
    {
        foreach (BreakerSwitch breaker in allBreakers)
        {
            if (breaker != null)
                breaker.SetState(false);
        }

        Debug.Log("Electrical fault! All breakers reset OFF.");
    }
}