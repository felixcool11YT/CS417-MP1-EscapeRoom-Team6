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

    [SerializeField] private DepartureStationController departureStation;
    private bool authorizationComplete;
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
        if (isUnlocked || authorizationComplete)
            return;

        itemsCollected++;

        if (progressText != null)
            progressText.text = $"AUTHORIZATION: {itemsCollected}/{itemsRequired}";

        if (itemsCollected >= itemsRequired)
        {
            authorizationComplete = true;
            UnlockDoor();
            if (departureStation != null)
                departureStation.BeginPreparation();
            else
                Debug.LogError("Final door has no departure station assigned.");
        }
    }
    public void CompleteDeparture()
    {
        if (!authorizationComplete || departureStation == null || !departureStation.IsComplete) return;
        if (progressText != null)
            progressText.text = "DEPARTURE READY\nHATCH RELEASED";
    }

    private void UnlockDoor()
    {
        if (isUnlocked)
            return;

        isUnlocked = true;

        if (progressText != null)
            progressText.text = "ACCESS GRANTED\nPREPARE IN AIRLOCK";

        if (doorAnimator != null)
            doorAnimator.SetTrigger("Open");

        if (unlockSound != null)
            unlockSound.Play();

        if (exitArea != null)
            exitArea.SetActive(true);

    }
}
