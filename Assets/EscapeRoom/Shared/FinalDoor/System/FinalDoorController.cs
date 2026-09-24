using UnityEngine;
using TMPro;

public class FinalDoorController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text progressText;

    [SerializeField]
    private Animator doorAnimator;

    [SerializeField]
    private GameObject winPanel;

    [SerializeField]
    private AudioSource unlockSound;

    [SerializeField]
    private ParticleSystem unlockParticles;

    [SerializeField]
    private GameObject exitArea;

    private bool isUnlocked = false;
    private int itemsCollected;

    [SerializeField] private int itemsRequired = 3;

    private void Start()
    {
        if (progressText != null)
            progressText.text = "AUTHORIZATION: 0/3";

        if (winPanel != null)
            winPanel.SetActive(false);

        if (exitArea != null)
            exitArea.SetActive(false);
    }

    public void RegisterItem()
    {
        if (isUnlocked)
            return;

        itemsCollected++;

        if (progressText != null)
            progressText.text = $"AUTHORIZATION: {itemsCollected}/{itemsRequired}";

        if (itemsCollected >= itemsRequired)
            UnlockDoor();
    }
    private void UnlockDoor()
    {
        if (isUnlocked)
            return;

        isUnlocked = true;

        GameFlowController gameFlow = FindFirstObjectByType<GameFlowController>();
        if (gameFlow != null)
            gameFlow.CompleteGame();

        if (doorAnimator != null)
            doorAnimator.SetTrigger("Open");

        if (unlockSound != null)
            unlockSound.Play();

        if (unlockParticles != null)
            unlockParticles.Play();

        if (exitArea != null)
            exitArea.SetActive(true);

        if (winPanel != null)
            winPanel.SetActive(true);
    }
}
