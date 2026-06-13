using UnityEngine;
using Vuforia;

public class TargetStabilizer : MonoBehaviour
{
    [Header("Stabilization & Locking Settings")]
    [Tooltip("If true, once a stable position is locked on the first scan, it is never reset or updated again, even if tracking is lost and found.")]
    public bool lockOnFirstScanOnly = true;

    [Tooltip("If true, once the target has been scanned and locked, the visuals will remain visible and interactive even if the card is moved away or tracking is lost.")]
    public bool keepVisibleWhenTrackingLost = true;

    [Tooltip("Time in seconds of tracking stability required before locking the pose.")]
    public float stabilizationTime = 0.5f;

    [Tooltip("If lockOnFirstScanOnly is false, this is the distance in meters the card must move to update the position.")]
    public float positionLockThreshold = 0.04f;

    [Tooltip("If lockOnFirstScanOnly is false, this is the angle in degrees the card must rotate to update the rotation.")]
    public float rotationLockThreshold = 10f;

    [Header("Interpolation Settings")]
    public float lerpSpeed = 8f;
    public float slerpSpeed = 8f;

    private enum TrackingState
    {
        Unobserved,
        Stabilizing,
        Locked,
        CatchingUp
    }

    // Static variables persist in memory across scene loads/reloads
    private static bool s_hasBeenLockedGlobal = false;
    private static Vector3 s_lockedPositionGlobal;
    private static Quaternion s_lockedRotationGlobal;

    private TrackingState currentState = TrackingState.Unobserved;
    private ObserverBehaviour observerBehaviour;
    private Transform originalParent;

    private Vector3 lockedPosition;
    private Quaternion lockedRotation;
    private Vector3 initialLocalScale;

    private float stabilizationTimer;
    private bool isTracked;
    private bool hasBeenLocked;

    // Caching for movement stability check
    private Vector3 lastTargetPos;
    private Quaternion lastTargetRot;

    void Awake()
    {
        // Cache the original local scale so we can keep it constant at runtime
        initialLocalScale = transform.localScale;
    }

    void Start()
    {
        originalParent = transform.parent;
        observerBehaviour = GetComponentInParent<ObserverBehaviour>();
        if (observerBehaviour != null)
        {
            observerBehaviour.OnTargetStatusChanged += OnStatusChanged;
        }
        else
        {
            Debug.LogWarning("TargetStabilizer: No ObserverBehaviour found in parent GameObjects.");
        }

        // Restore locked pose from previous scene load if lockOnFirstScanOnly is true
        if (lockOnFirstScanOnly && s_hasBeenLockedGlobal)
        {
            lockedPosition = s_lockedPositionGlobal;
            lockedRotation = s_lockedRotationGlobal;
            hasBeenLocked = true;
            currentState = TrackingState.Locked;
            isTracked = true; // Pretend tracked so it doesn't get disabled
            
            // Set position/rotation and immediately detach from target parent to stay in world space
            transform.position = lockedPosition;
            transform.rotation = lockedRotation;
            LockInWorldSpace();
            
            // Force all child components (renderers, canvases, colliders) to be visible/active
            SetChildComponentsEnabled(true);
            
            Debug.Log($"TargetStabilizer: Restored persistent locked pose at {lockedPosition}. No scanning required.");
        }
        else
        {
            if (observerBehaviour != null)
            {
                UpdateTrackingState(observerBehaviour.TargetStatus);
            }
        }
    }

    public static void ResetPersistentPose()
    {
        s_hasBeenLockedGlobal = false;
        s_lockedPositionGlobal = Vector3.zero;
        s_lockedRotationGlobal = Quaternion.identity;
        Debug.Log("TargetStabilizer: Persistent locked pose cleared/reset.");
    }

    void OnDestroy()
    {
        if (observerBehaviour != null)
        {
            observerBehaviour.OnTargetStatusChanged -= OnStatusChanged;
        }
    }

    private void OnStatusChanged(ObserverBehaviour behaviour, TargetStatus targetStatus)
    {
        UpdateTrackingState(targetStatus);
    }

    private void UpdateTrackingState(TargetStatus targetStatus)
    {
        bool newTracked = targetStatus.Status == Status.TRACKED || targetStatus.Status == Status.EXTENDED_TRACKED;

        if (newTracked && !isTracked)
        {
            isTracked = true;
            if (lockOnFirstScanOnly && hasBeenLocked)
            {
                currentState = TrackingState.Locked;
                LockInWorldSpace();
            }
            else
            {
                // Reparent to follow target during stabilization
                ReparentToOriginal();
                currentState = TrackingState.Stabilizing;
                stabilizationTimer = 0f;
            }
            Debug.Log("TargetStabilizer: Target detected.");
        }
        else if (!newTracked && isTracked)
        {
            isTracked = false;
            if (!lockOnFirstScanOnly)
            {
                currentState = TrackingState.Unobserved;
                ReparentToOriginal();
            }
            Debug.Log("TargetStabilizer: Target lost.");
        }
    }

    void LateUpdate()
    {
        // Enforce the initial local scale to prevent depth-based size changes
        transform.localScale = initialLocalScale;

        // If tracking is lost but we want to keep the content visible
        if (!isTracked)
        {
            if (keepVisibleWhenTrackingLost && hasBeenLocked)
            {
                // Force components to remain active
                SetChildComponentsEnabled(true);
                // Hold the locked pose
                transform.position = lockedPosition;
                transform.rotation = lockedRotation;
            }
            return;
        }

        Transform targetSource = originalParent != null ? originalParent : transform.parent;
        if (targetSource == null) return;

        Vector3 targetPos = targetSource.position;
        Quaternion targetRot = targetSource.rotation;

        switch (currentState)
        {
            case TrackingState.Unobserved:
                if (lockOnFirstScanOnly && hasBeenLocked)
                {
                    currentState = TrackingState.Locked;
                    LockInWorldSpace();
                }
                else
                {
                    ReparentToOriginal();
                    currentState = TrackingState.Stabilizing;
                    stabilizationTimer = 0f;
                }
                break;

            case TrackingState.Stabilizing:
                // Smoothly transition content to target pose
                transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, slerpSpeed * Time.deltaTime);

                // Calculate motion since last frame
                float frameMovement = Vector3.Distance(targetPos, lastTargetPos);
                float frameRotation = Quaternion.Angle(targetRot, lastTargetRot);

                // If target is moving too fast (not steady on flat surface), reset timer
                if (frameMovement > 0.005f || frameRotation > 1.0f)
                {
                    stabilizationTimer = 0f;
                }
                else
                {
                    stabilizationTimer += Time.deltaTime;
                }

                // Cache current state for the next frame's comparison
                lastTargetPos = targetPos;
                lastTargetRot = targetRot;

                if (stabilizationTimer >= stabilizationTime)
                {
                    lockedPosition = transform.position;
                    lockedRotation = transform.rotation;
                    currentState = TrackingState.Locked;
                    hasBeenLocked = true;
                    LockInWorldSpace();
                    Debug.Log($"TargetStabilizer: Pose permanently locked on first scan at position: {lockedPosition}");
                }
                break;

            case TrackingState.Locked:
                // Keep locked in place
                transform.position = lockedPosition;
                transform.rotation = lockedRotation;

                if (!lockOnFirstScanOnly)
                {
                    // Check if target moved past the breakout thresholds
                    float distance = Vector3.Distance(targetPos, lockedPosition);
                    float angle = Quaternion.Angle(targetRot, lockedRotation);

                    if (distance > positionLockThreshold || angle > rotationLockThreshold)
                    {
                        ReparentToOriginal();
                        currentState = TrackingState.CatchingUp;
                        Debug.Log("TargetStabilizer: Threshold exceeded. Catching up...");
                    }
                }
                break;

            case TrackingState.CatchingUp:
                transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, slerpSpeed * Time.deltaTime);

                // Relock once close enough
                if (Vector3.Distance(transform.position, targetPos) < 0.005f && Quaternion.Angle(transform.rotation, targetRot) < 2f)
                {
                    lockedPosition = transform.position;
                    lockedRotation = transform.rotation;
                    currentState = TrackingState.Locked;
                    LockInWorldSpace();
                    Debug.Log("TargetStabilizer: Pose Relocked.");
                }
                break;
        }
    }

    private void LockInWorldSpace()
    {
        if (transform.parent != null)
        {
            // Detach from target parent to stay fixed in world space coordinates
            transform.SetParent(null, true);
        }

        // Save locked pose to global variables for cross-scene persistence
        if (lockOnFirstScanOnly)
        {
            s_hasBeenLockedGlobal = true;
            s_lockedPositionGlobal = lockedPosition;
            s_lockedRotationGlobal = lockedRotation;
        }
    }

    private void ReparentToOriginal()
    {
        if (transform.parent != originalParent && originalParent != null)
        {
            // Re-attach to follow the image target again
            transform.SetParent(originalParent, true);
        }
    }

    private void SetChildComponentsEnabled(bool enable)
    {
        foreach (var rendererComponent in GetComponentsInChildren<Renderer>(true))
        {
            if (rendererComponent.enabled != enable)
                rendererComponent.enabled = enable;
        }
        foreach (var colliderComponent in GetComponentsInChildren<Collider>(true))
        {
            if (colliderComponent.enabled != enable)
                colliderComponent.enabled = enable;
        }
        foreach (var canvasComponent in GetComponentsInChildren<Canvas>(true))
        {
            if (canvasComponent.enabled != enable)
                canvasComponent.enabled = enable;
        }
    }
}
