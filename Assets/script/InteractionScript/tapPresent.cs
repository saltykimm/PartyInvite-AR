using UnityEngine;

public class tapPresent : TapInteractable
{
    [Header("UI Panel")]
    [SerializeField] private GameObject uiPanel;

    protected override void OnTap()
    {
        if (uiPanel == null)
        {
            Debug.LogWarning($"[{gameObject.name}] tapPresent: uiPanel is not assigned!");
            return;
        }

        if (uiPanel.activeSelf)
        {
            Debug.Log("CLOSE UI");
            uiPanel.SetActive(false);
        }
        else
        {
            Debug.Log("OPEN UI");
            uiPanel.SetActive(true);
        }
    }
}