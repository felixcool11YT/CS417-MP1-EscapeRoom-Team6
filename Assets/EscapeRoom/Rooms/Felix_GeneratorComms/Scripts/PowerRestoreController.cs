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

    [Header("Ambient Lighting")]
    public Color powerOffAmbient = new Color(0.10f, 0.14f, 0.24f);
    public Color powerOnAmbient = new Color(0.25f, 0.29f, 0.36f);
    public float ambientFadeTime = 1.5f;

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

    private void Start()
    {
        RenderSettings.ambientLight = powerOffAmbient;
    }
    public void RestorePower()
    {
        if (restored)
            return;

        restored = true;

        StartCoroutine(FadeAmbientLighting());

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

    private IEnumerator FadeAmbientLighting()
    {
        Color startColor = RenderSettings.ambientLight;
        float elapsed = 0f;

        while (elapsed < ambientFadeTime)
        {
            elapsed += Time.deltaTime;

            RenderSettings.ambientLight = Color.Lerp(
                startColor,
                powerOnAmbient,
                elapsed / ambientFadeTime
            );

            yield return null;
        }

        RenderSettings.ambientLight = powerOnAmbient;
    }
}