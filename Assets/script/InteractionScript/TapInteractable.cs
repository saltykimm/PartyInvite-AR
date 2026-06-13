using UnityEngine;
using UnityEngine.EventSystems;

public abstract class TapInteractable : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("If true, taps on 3D objects will be ignored if they land on a UI element. Turn this off if your UI is blocking clicks on mobile.")]
    public bool ignoreTouchesOverUI = false;

    [Tooltip("Optional: Explicitly assign a camera. If left empty, the script will automatically find the active AR camera at runtime.")]
    public Camera arCamera;

    private int lastTriggerFrame = -1;

    void Start()
    {
        // Warn if no camera is available at start, but we will dynamically look it up anyway
        if (arCamera == null && Camera.main == null && FindFirstObjectByType<Camera>() == null)
        {
            Debug.LogWarning($"[{gameObject.name}] TapInteractable: No Camera found in the scene during Start. Will attempt dynamic lookup on tap.");
        }
    }

    void Update()
    {
        // 1. Mobile Touch support
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Touch touch = Input.GetTouch(0);

            // Ignore if touching on UI (safely check for null EventSystem)
            if (ignoreTouchesOverUI && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                Debug.Log($"[{gameObject.name}] Touch ignored: finger is over UI element.");
                return;
            }

            Debug.Log($"[{gameObject.name}] Mobile touch detected: Raycasting...");
            TryHit(touch.position);
        }

        // 2. Mouse Click support (Always runs so Editor click works on touchscreen laptops)
        if (Input.GetMouseButtonDown(0))
        {
            // Ignore if clicking on UI (safely check for null EventSystem)
            if (ignoreTouchesOverUI && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log($"[{gameObject.name}] Mouse click ignored: pointer is over UI element.");
                return;
            }

            Debug.Log($"[{gameObject.name}] Mouse click detected: Raycasting...");
            TryHit(Input.mousePosition);
        }
    }

    void TryHit(Vector2 pos)
    {
        // Frame-rate debounce: Prevent executing twice in the same frame (e.g. touch + mouse click trigger)
        if (Time.frameCount == lastTriggerFrame) return;
        lastTriggerFrame = Time.frameCount;

        // Resolve active camera dynamically to handle Vuforia camera switching
        Camera activeCamera = arCamera;
        if (activeCamera == null || !activeCamera.gameObject.activeInHierarchy)
        {
            activeCamera = Camera.main;
            if (activeCamera == null || !activeCamera.gameObject.activeInHierarchy)
            {
                Camera[] cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
                foreach (Camera cam in cameras)
                {
                    if (cam.gameObject.activeInHierarchy)
                    {
                        activeCamera = cam;
                        break;
                    }
                }
            }
        }

        if (activeCamera == null)
        {
            Debug.LogError($"[{gameObject.name}] TapInteractable: Raycast failed because no active Camera could be found in the scene.");
            return;
        }

        Ray ray = activeCamera.ScreenPointToRay(pos);
        
        // RaycastAll detects all colliders along the ray path.
        // This prevents other colliders (like a flat Plane or ImageTarget mesh) from obstructing the button.
        RaycastHit[] hits = Physics.RaycastAll(ray);

        if (hits.Length > 0)
        {
            // Sort hits by distance (closest first)
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            bool registeredTap = false;
            foreach (RaycastHit hit in hits)
            {
                Debug.Log($"[{gameObject.name}] Raycast hit in path: {hit.transform.name} (Collider: {hit.collider.GetType().Name}, Distance: {hit.distance}) using Camera: {activeCamera.name}");

                if (hit.transform == transform || hit.transform.IsChildOf(transform))
                {
                    Debug.Log($"[{gameObject.name}] Tap successfully registered on target object!");
                    OnTap();
                    registeredTap = true;
                    break; // Exit loop after registering the tap
                }
            }

            if (!registeredTap)
            {
                Debug.Log($"[{gameObject.name}] Raycast hit other objects, but none matched this button.");
            }
        }
        else
        {
            Debug.Log($"[{gameObject.name}] Raycast did not hit any 3D colliders.");
        }
    }

    protected abstract void OnTap();
}