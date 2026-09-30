using UnityEngine;

public partial class DepartureStationController : MonoBehaviour
{
private enum Stage
{
    AwaitingAuthorization,
    Packing,
    Purging,
    Powering,
    Complete
}

private Stage currentStage = Stage.AwaitingAuthorization;
public bool IsComplete => currentStage == Stage.Complete;
public void BeginPreparation()
{
    if(currentStage != Stage.AwaitingAuthorization)
    {
        return;
    }
    currentStage = Stage.Packing;
    RefreshStage();
    Debug.Log("Departure station authorized. Pack the exit supplies.");

}
public void SelectFilter(int filterIndex)
    {
        if(currentStage != Stage.Purging)
        {
            return;
        }
        if(filterIndex == filterStep)
        {
            filterStep++;
        } else {
            filterStep = 0;
            RejectInput();
            return;
        }
        if(filterStep == 3)
        {
            currentStage = Stage.Powering;
        }
        RefreshStage();
    }
}
