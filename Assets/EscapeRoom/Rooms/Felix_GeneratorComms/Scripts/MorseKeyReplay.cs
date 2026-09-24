using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections;

public class MorseKeyReplay : MonoBehaviour
{
    [Header("Interaction")]
    public XRSimpleInteractable interactable;

    [Header("Key Arm")]
    public Transform keyArm;

    [Header("Radio")]
    public RadioDialController radioDial;
    public AudioSource radioAudio;
    public AudioClip morseReplayClip;

    [Header("Key Audio")]
    public AudioSource keyClickAudio;

    [Header("Animation")]
    public float pressTime = 0.10f;
    public float holdTime = 0.08f;

    private readonly Vector3 restEuler =
        new Vector3(-90f, 0f, 47.846f);

    private readonly Vector3 pressedEuler =
        new Vector3(-98.809f, 169.431f, -120.81f);

    private bool busy = false;

    private void Start()
    {
        if (interactable == null)
            interactable = GetComponent<XRSimpleInteractable>();

        if (interactable != null)
            interactable.selectEntered.AddListener(OnPressed);

        if (keyArm != null)
            keyArm.localRotation = Quaternion.Euler(restEuler);
    }

    private void OnDestroy()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnPressed);
    }

    private void OnPressed(SelectEnterEventArgs args)
    {
        if (!busy)
            StartCoroutine(PressKey());
    }

    private IEnumerator PressKey()
    {
        busy = true;

        Quaternion rest = Quaternion.Euler(restEuler);
        Quaternion pressed = Quaternion.Euler(pressedEuler);

        float t = 0f;

        while (t < pressTime)
        {
            t += Time.deltaTime;

            keyArm.localRotation = Quaternion.Slerp(
                rest,
                pressed,
                t / pressTime
            );

            yield return null;
        }

        if (keyClickAudio != null)
            keyClickAudio.Play();

        if (radioDial != null &&
            radioDial.IsPowered &&
            radioDial.CurrentStation == 2 &&
            radioAudio != null &&
            morseReplayClip != null)
        {
            radioAudio.Stop();
            radioAudio.clip = morseReplayClip;
            radioAudio.loop = false;
            radioAudio.Play();
        }

        yield return new WaitForSeconds(holdTime);

        t = 0f;

        while (t < pressTime)
        {
            t += Time.deltaTime;

            keyArm.localRotation = Quaternion.Slerp(
                pressed,
                rest,
                t / pressTime
            );

            yield return null;
        }

        keyArm.localRotation = rest;

        busy = false;
    }
}