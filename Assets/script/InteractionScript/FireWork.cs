using UnityEngine;

public class FireworkCannon : TapInteractable
{
    public ParticleSystem firework;

    protected override void OnTap()
    {
        Debug.Log("FIRE IN THE HOLE");
        Fire();
    }

    void Fire()
    {
        firework.Play();
    }
}