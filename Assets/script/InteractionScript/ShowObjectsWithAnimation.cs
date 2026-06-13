using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShowObjectsWithAnimation : TapInteractable
{
    [Header("3D Button Reference")]
    [SerializeField] private Renderer buttonRenderer;
    [SerializeField] private Color activeColor = new Color(0.18f, 0.8f, 0.44f); // Premium Emerald Green
    [SerializeField] private Color inactiveColor = Color.white;

    [Header("Information UI")]
    [Tooltip("UI element that tells what the button shows. It will hide when the button is active.")]
    [SerializeField] private GameObject infoUiElement;

    [Header("Target Objects")]
    [SerializeField] private GameObject[] targetObjects;

    [Header("Animation Settings")]
    [SerializeField] private float fallHeight = 3.0f;
    [SerializeField] private float duration = 0.6f;
    [SerializeField] private AnimationCurve fallCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector3[] originalPositions;
    private bool isShown = false;
    private Material buttonMaterial;
    private Coroutine[] animationCoroutines;

    void Start()
    {
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

        // Show info UI initially since objects start hidden
        if (infoUiElement != null)
        {
            infoUiElement.SetActive(true);
        }

        // Cache original positions and hide objects initially
        if (targetObjects != null)
        {
            originalPositions = new Vector3[targetObjects.Length];
            animationCoroutines = new Coroutine[targetObjects.Length];

            for (int i = 0; i < targetObjects.Length; i++)
            {
                if (targetObjects[i] != null)
                {
                    originalPositions[i] = targetObjects[i].transform.localPosition;
                    targetObjects[i].SetActive(false);
                }
            }
        }
    }

    protected override void OnTap()
    {
        OnToggleTriggered();
    }

    public void OnToggleTriggered()
    {
        isShown = !isShown;

        // Update Button Visuals (3D Renderer)
        if (buttonMaterial != null)
        {
            buttonMaterial.color = isShown ? activeColor : inactiveColor;
        }

        // Toggle information UI (hide when objects are shown, show when hidden)
        if (infoUiElement != null)
        {
            infoUiElement.SetActive(!isShown);
        }

        // Handle objects visibility and animation
        for (int i = 0; i < targetObjects.Length; i++)
        {
            if (targetObjects[i] == null) continue;

            // Stop any running animation for this object
            if (animationCoroutines[i] != null)
            {
                StopCoroutine(animationCoroutines[i]);
            }

            if (isShown)
            {
                targetObjects[i].SetActive(true);
                animationCoroutines[i] = StartCoroutine(AnimateFall(targetObjects[i].transform, originalPositions[i], i));
            }
            else
            {
                targetObjects[i].SetActive(false);
                // Reset to original position when hidden
                targetObjects[i].transform.localPosition = originalPositions[i];
            }
        }
    }

    private IEnumerator AnimateFall(Transform objTransform, Vector3 targetLocalPos, int index)
    {
        Vector3 startLocalPos = targetLocalPos + Vector3.up * fallHeight;
        objTransform.localPosition = startLocalPos;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curveValue = fallCurve.Evaluate(t);
            
            if (objTransform != null)
            {
                objTransform.localPosition = Vector3.LerpUnclamped(startLocalPos, targetLocalPos, curveValue);
            }
            yield return null;
        }

        if (objTransform != null)
        {
            objTransform.localPosition = targetLocalPos;
        }
        animationCoroutines[index] = null;
    }
}
