using UnityEngine;
using UnityEngine.Events;

public class WaterLeakPuzzle : MonoBehaviour
{
    [Header("Leaks")]
    public ParticleSystem leakA;
    public ParticleSystem leakB;

    [Header("Leak Strength")]
    public float fullLeakRate = 80f;
    public float sealedLeakRate = 18f;

    [Header("Progress")]
    public UnityEvent onBothLeaksSealed;

    private bool leakASealed = false;
    private bool leakBSealed = false;
    private bool sealStageComplete = false;

    [Header("Repair Visuals")]
    public GameObject sealPatchA;
    public GameObject sealPatchB;


    [Header("Leak Audio")]
    public AudioSource leakAAudio;
    public AudioSource leakBAudio;

    public float fullLeakVolume = 0.45f;
    public float sealedLeakVolume = 0.12f;

    public AudioSource sealFXAudio;
    public ParticleSystem glueFXA;
    public ParticleSystem glueFXB;

    void Start()
    {
        SetLeakRate(leakA, fullLeakRate);
        SetLeakRate(leakB, fullLeakRate);

        if (leakAAudio != null)
        {
            leakAAudio.volume = fullLeakVolume;

            if (!leakAAudio.isPlaying)
                leakAAudio.Play();
        }

        if (leakBAudio != null)
        {
            leakBAudio.volume = fullLeakVolume;

            if (!leakBAudio.isPlaying)
                leakBAudio.Play();
        }

        if (sealPatchA != null)
            sealPatchA.SetActive(false);

        if (sealPatchB != null)
            sealPatchB.SetActive(false);

        if (leakA != null)
            leakA.Play();

        if (leakB != null)
            leakB.Play();
    }

    public void SealLeakA()
    {
        if (leakASealed)
            return;

        leakASealed = true;

        SetLeakRate(leakA, sealedLeakRate);

        if (leakAAudio != null)
            leakAAudio.volume = sealedLeakVolume;

        if (glueFXA != null)
            glueFXA.Play();

        if (sealFXAudio != null)
            sealFXAudio.Play();

        if (sealPatchA != null)
            sealPatchA.SetActive(true);

        CheckBothSealed();
    }

    public void SealLeakB()
    {
        if (leakBSealed)
            return;

        leakBSealed = true;

        SetLeakRate(leakB, sealedLeakRate);

        if (leakBAudio != null)
            leakBAudio.volume = sealedLeakVolume;

        if (glueFXA != null)
            glueFXA.Play();

        if (sealFXAudio != null)
            sealFXAudio.Play();

        if (sealPatchB != null)
            sealPatchB.SetActive(true);

        CheckBothSealed();
    }

    private void CheckBothSealed()
    {
        if (leakASealed &&
            leakBSealed &&
            !sealStageComplete)
        {
            sealStageComplete = true;

            onBothLeaksSealed.Invoke();
        }
    }

    public void StopAllWater()
    {
        if (leakA != null)
            leakA.Stop(
                true,
                ParticleSystemStopBehavior.StopEmitting
            );

        if (leakB != null)
            leakB.Stop(
                true,
                ParticleSystemStopBehavior.StopEmitting
            );

        if (leakAAudio != null)
            leakAAudio.Stop();

        if (leakBAudio != null)
            leakBAudio.Stop();
    }

    private void SetLeakRate(
        ParticleSystem system,
        float rate)
    {
        if (system == null)
            return;

        ParticleSystem.EmissionModule emission =
            system.emission;

        emission.rateOverTime = rate;
    }
}