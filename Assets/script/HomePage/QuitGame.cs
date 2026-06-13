using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void Quit()
    {
        Debug.Log("Quit Game requested.");

        #if UNITY_EDITOR
        // Exits play mode in the Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // Quits the actual built application
        Application.Quit();
        #endif
    }
}
