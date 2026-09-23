using UnityEngine;
using TMPro;

public class PuzzleProgressDisplay : MonoBehaviour

{
    [SerializeField]
    private AudioSource negativeFeedbackAudio;

    [SerializeField]
    private AudioSource positiveFeedbackAudio;

    [SerializeField]
    private TMP_Text progressText;

    public void ShowProgress(int completedSteps)
    {
        progressText.text = "Steps completed: " + completedSteps.ToString() + " of 3";
    }

    public void ShowWrongOrder()
    {
        progressText.text = "WRONG ORDER, TRY AGAIN!";
        // Todo: Add negative feedback music
        negativeFeedbackAudio.Play();
    }
    public void ShowComplete()
    {
        progressText.text = "Puzzle complete! Keycard dispensed.";
        positiveFeedbackAudio.Play();
        //Todo: once we know what deliverables teammates have,
        // we can specify what's next so they know they have 2 more things left
    }
}