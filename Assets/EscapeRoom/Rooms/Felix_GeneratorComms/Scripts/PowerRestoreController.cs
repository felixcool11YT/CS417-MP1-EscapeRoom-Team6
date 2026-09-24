using UnityEngine;
using System.Collections;

public class PowerRestoreController : MonoBehaviour
{
    [Header("Lighting")]
    public GameObject[] lightsToEnable;
    public GameObject[] lightsToDisable;

    [Header("Audio")]
    public AudioSource powerOnAudio;
    public AudioSource generatorHum;

    [Header("Next Puzzle")]
    public GameObject nextPuzzleRoot;

    private bool restored = false;

    public RadioDialController radioDial;


    private IEnumerator StartHumAfterStartup()
    {
        float delay = 0.75f;

        if (powerOnAudio != null && powerOnAudio.clip != null)
            delay = powerOnAudio.clip.length;

        yield return new WaitForSeconds(delay);

        generatorHum.Play();

    }
    public void RestorePower()
    {
        if (restored)
            return;

        restored = true;

        foreach (GameObject obj in lightsToDisable)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        foreach (GameObject obj in lightsToEnable)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        if (radioDial != null)
            radioDial.SetPowered(true);

        if (powerOnAudio != null)
            powerOnAudio.Play();

        if (generatorHum != null)
            StartCoroutine(StartHumAfterStartup());

        if (nextPuzzleRoot != null)
            nextPuzzleRoot.SetActive(true);

        Debug.Log("POWER RESTORED");
    }
}