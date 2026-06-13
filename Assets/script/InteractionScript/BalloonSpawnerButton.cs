using UnityEngine;

public class BalloonSpawnerButton : TapInteractable
{
    [Header("Spawner References")]
    [SerializeField] private balloonSpawner[] spawners;

    [Header("3D Button Visuals")]
    [SerializeField] private Renderer buttonRenderer;
    [SerializeField] private Color activeColor = new Color(0.18f, 0.8f, 0.44f); // Emerald Green
    [SerializeField] private Color inactiveColor = new Color(0.9f, 0.3f, 0.2f); // Alizarin Red (Contrast)

    [Header("Information UI")]
    [Tooltip("UI element that tells what the button shows. It will hide when the button is active.")]
    [SerializeField] private GameObject infoUiElement;

    private bool isActive = false;
    private Material buttonMaterial;

    void Start()
    {
        // Auto-find spawners if array is empty
        if (spawners == null || spawners.Length == 0)
        {
            spawners = FindObjectsByType<balloonSpawner>(FindObjectsSortMode.None);
        }

        // Setup 3D Renderer if available
        if (buttonRenderer == null)
        {
            buttonRenderer = GetComponent<Renderer>();
        }

        if (buttonRenderer != null)
        {
            buttonMaterial = buttonRenderer.material;
            if (buttonMaterial != null)
            {
                buttonMaterial.color = inactiveColor;
            }
        }

        // Show info UI initially since spawners start inactive
        if (infoUiElement != null)
        {
            infoUiElement.SetActive(true);
        }
    }

    protected override void OnTap()
    {
        ToggleSpawning();
    }

    public void ToggleSpawning()
    {
        isActive = !isActive;

        // Notify Spawners of new state
        if (spawners != null)
        {
            foreach (var spawner in spawners)
            {
                if (spawner != null)
                {
                    spawner.SetToggleState(isActive);
                }
            }
        }

        // Update button color
        if (buttonMaterial != null)
        {
            buttonMaterial.color = isActive ? activeColor : inactiveColor;
        }

        // Toggle information UI (hide when active/spawning, show when inactive)
        if (infoUiElement != null)
        {
            infoUiElement.SetActive(!isActive);
        }
    }
}
