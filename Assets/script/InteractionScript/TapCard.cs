using UnityEngine;

public class TapCard : TapInteractable
{
    [Header("UI Reference")]
    [SerializeField] private GameObject uiPanel;

    protected override void OnTap()
    {
        if (uiPanel == null)
        {
            Debug.LogWarning($"[{gameObject.name}] TapCard: uiPanel is not assigned!");
            return;
        }

        bool newState = !uiPanel.activeSelf;
        uiPanel.SetActive(newState);
        Debug.Log($"[{gameObject.name}] Card tapped: set uiPanel to {newState}");
    }
}
