using UnityEngine;
using System.Collections;

public class GeneratorRewardCompartment : MonoBehaviour
{
    [Header("Hatch")]
    public Transform hatchDoor;

    public Vector3 closedPosition =
        new Vector3(-0.159f, 0.551f, -0.022f);

    public Vector3 closedRotation =
        new Vector3(0f, 90f, 0f);

    public Vector3 openPosition =
        new Vector3(-0.098f, 0.636f, -0.039f);

    public Vector3 openRotation =
        new Vector3(60f, 90f, 0f);

    public float hatchSpeed = 3f;

    [Header("Reward")]
    public GameObject authorizationCapsule;
    public Transform capsuleRevealPoint;

    [Header("Timing")]
    public float openWait = 0.4f;
    public float revealWait = 0.6f;
    public float closeWait = 0.8f;

    private bool activated = false;

    private void Start()
    {
        if (hatchDoor != null)
        {
            hatchDoor.localPosition = closedPosition;
            hatchDoor.localRotation = Quaternion.Euler(closedRotation);
        }

        if (authorizationCapsule != null)
            authorizationCapsule.SetActive(false);
    }

    public void RevealReward()
    {
        if (activated)
            return;

        activated = true;
        StartCoroutine(RewardSequence());
    }

    private IEnumerator RewardSequence()
    {
        // OPEN
        yield return StartCoroutine(
            MoveHatch(openPosition, openRotation)
        );

        yield return new WaitForSeconds(openWait);

        // REVEAL CAPSULE
        if (authorizationCapsule != null)
        {
            authorizationCapsule.SetActive(true);

            if (capsuleRevealPoint != null)
            {
                authorizationCapsule.transform.position =
                    capsuleRevealPoint.position;

                authorizationCapsule.transform.rotation =
                    capsuleRevealPoint.rotation;
            }
        }

        yield return new WaitForSeconds(revealWait + closeWait);

        // CLOSE AGAIN
        yield return StartCoroutine(
            MoveHatch(closedPosition, closedRotation)
        );
    }

    private IEnumerator MoveHatch(
        Vector3 targetPosition,
        Vector3 targetRotation)
    {
        Quaternion targetRot = Quaternion.Euler(targetRotation);

        while (
            Vector3.Distance(hatchDoor.localPosition, targetPosition) > 0.001f ||
            Quaternion.Angle(hatchDoor.localRotation, targetRot) > 0.5f)
        {
            hatchDoor.localPosition = Vector3.Lerp(
                hatchDoor.localPosition,
                targetPosition,
                hatchSpeed * Time.deltaTime
            );

            hatchDoor.localRotation = Quaternion.Slerp(
                hatchDoor.localRotation,
                targetRot,
                hatchSpeed * Time.deltaTime
            );

            yield return null;
        }

        hatchDoor.localPosition = targetPosition;
        hatchDoor.localRotation = targetRot;
    }
}