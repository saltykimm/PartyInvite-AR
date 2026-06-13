using UnityEngine;
using UnityEngine.SceneManagement;

public class TapToLoadScene : TapInteractable
{
    [Header("Scene Loading Settings")]
    [Tooltip("The exact name of the scene you want to load when this object is tapped.")]
    [SerializeField] private string sceneName;

    protected override void OnTap()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning($"[{gameObject.name}] TapToLoadScene: No scene name specified!");
            return;
        }

        Debug.Log($"[{gameObject.name}] Tapped. Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
}
