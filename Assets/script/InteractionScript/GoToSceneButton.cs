using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToSceneButton : TapInteractable
{
    [Header("Navigation Settings")]
    [Tooltip("The name of the scene to load when this button is clicked/tapped (e.g. MainScene).")]
    [SerializeField] private string targetSceneName = "MainScene";

    // This handles 3D Taps (if the X button is a 3D Object in the scene)
    protected override void OnTap()
    {
        GoToScene();
    }

    // This can be hooked up to a UI Button's OnClick() event (if the X button is a UI Button)
    public void GoToScene()
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogWarning($"[{gameObject.name}] GoToSceneButton: targetSceneName is empty!");
            return;
        }

        Debug.Log($"[{gameObject.name}] Loading scene: {targetSceneName}");
        SceneManager.LoadScene(targetSceneName);
    }
}
