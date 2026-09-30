using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Collections;

public class TerminalCodePuzzle : MonoBehaviour
{
    [Header("Display")]
    public TMP_Text displayText;

    [Header("Code")]
    public string targetCode = "CS417";
    public int maxLength = 5;

    [Header("Audio")]
    public AudioSource keyAudio;
    public AudioSource correctAudio;
    public AudioSource wrongAudio;

    [Header("Success")]
    public UnityEvent onCorrectCode;

    private string currentInput = "";
    private bool solved = false;
    private Coroutine wrongFeedback;

    private void Start()
    {
        UpdateDisplay();
    }

    public void AddCharacter(string character)
    {
        if (solved)
            return;

        if (currentInput.Length >= maxLength)
            return;

        CancelWrongFeedback();
        currentInput += character;

        if (keyAudio != null)
            keyAudio.Play();

        UpdateDisplay();
    }

    public void ClearInput()
    {
        if (solved)
            return;

        CancelWrongFeedback();
        currentInput = "";
        UpdateDisplay();
    }

    public void SubmitCode()
    {
        if (solved)
            return;

        CancelWrongFeedback();
        if (currentInput == targetCode)
        {
            solved = true;

            if (correctAudio != null)
                correctAudio.Play();

            displayText.text =
                "COMM ACCESS\n\nACCESS GRANTED\nCS417";

            onCorrectCode?.Invoke();

            Debug.Log("Terminal code accepted.");
        }
        else
        {
            if (wrongAudio != null)
                wrongAudio.Play();

            wrongFeedback = StartCoroutine(WrongCodeFeedback());
        }
    }

    private void CancelWrongFeedback()
    {
        if (wrongFeedback == null) return;
        StopCoroutine(wrongFeedback);
        wrongFeedback = null;
    }

    private IEnumerator WrongCodeFeedback()
    {
        displayText.text =
            "COMM ACCESS\n\nACCESS DENIED";

        yield return new WaitForSeconds(1.0f);

        wrongFeedback = null;
        currentInput = "";
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        string shown = currentInput;

        while (shown.Length < maxLength)
            shown += "_";

        displayText.text =
            "COMM ACCESS\n\nSTATION ID:\n" + shown;
    }
}
