using UnityEngine;

public class tapGlass : TapInteractable
{
    public GameObject emptyCup;
    public GameObject fullCup;

    public AudioClip audioClip;

    private bool isFull = true;
    private Renderer parentRenderer;

    void Start()
    {
        parentRenderer = GetComponent<Renderer>();
        UpdateCup();
    }

    protected override void OnTap()
    {
        ToggleGlass();
    }

    void ToggleGlass()
    {
        isFull = !isFull;

        if (!isFull && audioClip != null)
        {
            AudioSource.PlayClipAtPoint(audioClip, transform.position);
        }

        UpdateCup();
    }

    void UpdateCup()
    {
        if (emptyCup != null)
        {
            emptyCup.SetActive(!isFull);
        }

        if (fullCup != null)
        {
            fullCup.SetActive(isFull);
        }

        // If the main model mesh renderer is on this parent object, toggle it
        if (parentRenderer != null)
        {
            parentRenderer.enabled = isFull;
        }
    }
}