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

    [SerializeField] private AudioSource placementFeedbackAudio;

    public void ShowProgress(int completedSteps)
    {
        progressText.text = "Steps completed: " + completedSteps.ToString() + " of 3";
        if (completedSteps > 0 && placementFeedbackAudio != null)
            placementFeedbackAudio.Play();
    }

    public void ShowWrongOrder()
    {
        progressText.text = "WRONG ORDER: REMOVE FILTERS\nREINSERT BLUE > GREEN > MAGENTA";

        negativeFeedbackAudio.Play();
    }
    public void ShowComplete()
    {
        progressText.text = "Puzzle complete! Keycard dispensed.";
        positiveFeedbackAudio.Play();

    }
}
