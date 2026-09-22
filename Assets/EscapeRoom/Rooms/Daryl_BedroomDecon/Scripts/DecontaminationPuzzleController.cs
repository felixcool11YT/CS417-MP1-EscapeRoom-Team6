using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
public class NewMonoBehaviourScript : MonoBehaviour
{
    private int currentStep = 0;
    private bool solved = false;

    [SerializeField]
    private XRSocketInteractor SocketParticulate;

    [SerializeField]
    private XRSocketInteractor SocketChemical;

    [SerializeField]
    private XRSocketInteractor SocketRadiation;

    private void Solve(SelectEnterEventArgs args)
    {   
        if(!solved)
        {
         if (currentStep == 0 && args.interactorObject is XRSocketInteractor socketP && socketP == SocketParticulate)
         {
             currentStep++;
         }
         else if (currentStep == 1 && args.interactorObject is XRSocketInteractor socketC && socketC == SocketChemical)
         {
             currentStep++;
         }
         else if (currentStep == 2 && args.interactorObject is XRSocketInteractor socketR && socketR == SocketRadiation)
         {
             currentStep++;
             solved = true;
             Solved();

         } else
         {
             currentStep = 0;
         }
        }
    }
    private void Solved()
    {
        Debug.Log("Puzzle solved!");
    }
    
}
