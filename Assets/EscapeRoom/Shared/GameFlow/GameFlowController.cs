using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowController : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private float startingTimeSeconds = 1200f;
    [SerializeField] private TMP_Text timerText;

    [Header("Collectibles")]
    [SerializeField] private int totalCollectibles = 3;
    [SerializeField] private TMP_Text inventoryText;

    [Header("Loss UI")]
    [SerializeField] private GameObject lossPanel;

    private readonly List<string> collectedItems = new List<string>();
    private float timeRemaining;
    private bool gameEnded;
    private int lastDisplayedSecond = -1;

    private void Start()
    {
        Time.timeScale = 1f;
        timeRemaining = startingTimeSeconds;

        if (lossPanel != null)
            lossPanel.SetActive(false);

        UpdateTimerText();
        UpdateInventoryText();
    }

    private void Update()
    {
        if (gameEnded)
            return;

        timeRemaining = Mathf.Max(0f, timeRemaining - Time.deltaTime);
        UpdateTimerText();

        if (timeRemaining <= 0f)
            LoseGame();
    }

    public void AddCollectible(string itemName)
    {
        if (gameEnded)
            return;

        collectedItems.Add(itemName);
        UpdateInventoryText();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CompleteGame()
    {
        gameEnded = true;
    }

    private void LoseGame()
    {
        gameEnded = true;

        if (lossPanel != null)
            lossPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    private void UpdateTimerText()
    {
        if (timerText == null)
            return;

        int totalSeconds = Mathf.CeilToInt(timeRemaining);

        if (totalSeconds == lastDisplayedSecond)
            return;

        lastDisplayedSecond = totalSeconds;
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        timerText.text = $"TIME: {minutes:00}:{seconds:00}";
    }

    private void UpdateInventoryText()
    {
        if (inventoryText == null)
            return;

        inventoryText.text = $"INVENTORY: {collectedItems.Count}/{totalCollectibles}";

        foreach (string item in collectedItems)
            inventoryText.text += $"\n• {item}";
    }
}
