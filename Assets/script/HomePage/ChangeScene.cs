using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadGame()
    {
        TargetStabilizer.ResetPersistentPose();
        SceneManager.LoadScene("MainScene");
    }
}