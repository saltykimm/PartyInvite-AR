using UnityEngine;

public class EatCookie : TapInteractable
{
    public AudioClip audioClip;
    protected override void OnTap()
    {
        AudioSource.PlayClipAtPoint(audioClip, transform.position);
        gameObject.SetActive(false);
    }
}