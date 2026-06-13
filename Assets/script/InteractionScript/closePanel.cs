using UnityEngine;

public class ClosePanel : MonoBehaviour
{
    public GameObject panel;
    public GameObject panel2;

    public void Close()
    {
        panel.SetActive(false);
        panel2.SetActive(true);
    }
}