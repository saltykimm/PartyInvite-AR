using UnityEngine;
using UnityEngine.UI;

public class HideToggle : MonoBehaviour
{
    public GameObject[] targetObject;
    public Image checkmark;         

    // item not hidden,yet.
    private bool isHidden = false;

    void Start()
    {
        
        UpdateVisual();
    }

    //OnClick.
    public void ToggleObject()
    {
        isHidden = !isHidden;

        UpdateVisual();
    }

    void UpdateVisual()
    {
        // Hide/show object
        foreach (GameObject obj in targetObject)
        {
            obj.SetActive(!isHidden);
        }

        // Show/hide checkmark
        checkmark.enabled = isHidden;
    }
}