using UnityEngine;
using Vuforia;

public class balloonSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private GameObject[] balloonPrefab;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnRange = 1f;

    private ObserverBehaviour observerBehaviour;
    private bool isTargetTracked = false;
    private bool isToggledOn = false;
    private bool isCurrentlySpawning = false;

    void Start()
    {
        observerBehaviour = GetComponentInParent<ObserverBehaviour>();
        if (observerBehaviour != null)
        {
            observerBehaviour.OnTargetStatusChanged += OnStatusChanged;
            
            // Initialize tracking state based on current Vuforia status
            var currentStatus = observerBehaviour.TargetStatus;
            isTargetTracked = currentStatus.Status == Status.TRACKED || currentStatus.Status == Status.EXTENDED_TRACKED;
            Debug.Log($"[{gameObject.name}] balloonSpawner started. Initial tracking status: {isTargetTracked}");
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] balloonSpawner: No ObserverBehaviour found in parent.");
        }
    }

    void OnDestroy()
    {
        if (observerBehaviour != null)
        {
            observerBehaviour.OnTargetStatusChanged -= OnStatusChanged;
        }
    }

    void OnStatusChanged(ObserverBehaviour behaviour, TargetStatus targetStatus)
    {
        isTargetTracked = targetStatus.Status == Status.TRACKED || targetStatus.Status == Status.EXTENDED_TRACKED;
        Debug.Log($"[{gameObject.name}] Vuforia status changed: tracking = {isTargetTracked}");
        UpdateSpawningState();
    }

    public void SetToggleState(bool toggle)
    {
        isToggledOn = toggle;
        Debug.Log($"[{gameObject.name}] Spawner toggle set to: {toggle}");
        UpdateSpawningState();
    }

    private void UpdateSpawningState()
    {
        bool shouldSpawn = isTargetTracked && isToggledOn;
        Debug.Log($"[{gameObject.name}] UpdateSpawningState: Tracked={isTargetTracked}, ToggledOn={isToggledOn} -> ShouldSpawn={shouldSpawn} (CurrentlySpawning={isCurrentlySpawning})");

        if (shouldSpawn && !isCurrentlySpawning)
        {
            isCurrentlySpawning = true;
            Debug.Log($"[{gameObject.name}] Starting InvokeRepeating for SpawnBalloon (Interval: {spawnInterval}s)");
            InvokeRepeating(nameof(SpawnBalloon), 0f, spawnInterval);
        }
        else if (!shouldSpawn && isCurrentlySpawning)
        {
            isCurrentlySpawning = false;
            Debug.Log($"[{gameObject.name}] Stopping InvokeRepeating for SpawnBalloon");
            CancelInvoke(nameof(SpawnBalloon));
        }
    }

    void SpawnBalloon()
    {
        if (balloonPrefab == null || balloonPrefab.Length == 0)
        {
            Debug.LogWarning($"[{gameObject.name}] SpawnBalloon: Balloon Prefabs array is empty!");
            return;
        }

        Vector3 spawnPosition = transform.position + new Vector3(
            Random.Range(-spawnRange, spawnRange),
            0,
            Random.Range(-spawnRange, spawnRange)
        );
        
        int randomIndex = Random.Range(0, balloonPrefab.Length);
        if (balloonPrefab[randomIndex] != null)
        {
            Debug.Log($"[{gameObject.name}] Spawning balloon prefab: {balloonPrefab[randomIndex].name} at {spawnPosition}");
            Instantiate(balloonPrefab[randomIndex], spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] SpawnBalloon: Prefab at index {randomIndex} is null!");
        }
    }
}
