using UnityEngine;

public class ToggleObject : MonoBehaviour
{
    public GameObject targetObject;

    private bool isVisible;

    void Start()
    {
        isVisible = targetObject.activeSelf;
    }

    public void Toggle()
    {
        isVisible = !isVisible;

        targetObject.SetActive(isVisible);
    }
}