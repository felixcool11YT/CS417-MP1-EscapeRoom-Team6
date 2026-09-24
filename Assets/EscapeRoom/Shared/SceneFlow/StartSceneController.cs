using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneController : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName = "MP1B_EscapeRoom";

    public void WakeUp()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameplaySceneName);
    }
}
