using UnityEngine;

[DefaultExecutionOrder(-100)] // Run before other scripts
public class CanvasCameraAssigner : MonoBehaviour
{
    private Canvas canvas;

    void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    void Start()
    {
        AssignCamera();
    }

    void OnEnable()
    {
        AssignCamera();
    }

    private void AssignCamera()
    {
        if (canvas == null) return;

        // Only assign to World Space canvases
        if (canvas.renderMode == RenderMode.WorldSpace && canvas.worldCamera == null)
        {
            Camera activeCamera = Camera.main;
            if (activeCamera == null)
            {
                activeCamera = FindFirstObjectByType<Camera>();
            }

            if (activeCamera != null)
            {
                canvas.worldCamera = activeCamera;
                Debug.Log($"[CanvasCameraAssigner] Successfully assigned active camera '{activeCamera.name}' to World Space Canvas '{gameObject.name}'");
            }
            else
            {
                Debug.LogWarning($"[CanvasCameraAssigner] Failed to find an active camera in the scene for Canvas '{gameObject.name}'");
            }
        }
    }
}
