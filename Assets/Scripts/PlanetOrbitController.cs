using UnityEngine;

public class PlanetOrbitController : MonoBehaviour
{
    [SerializeField]

    private float orbitSpeed = 20;


    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, orbitSpeed*Time.deltaTime, 0);
        
    }
}
