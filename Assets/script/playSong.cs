using UnityEngine;
using Vuforia;

public class playSong : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private AudioSource audioSource;
    private ObserverBehaviour observerBehaviour;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        observerBehaviour = GetComponent<ObserverBehaviour>();

        observerBehaviour.OnTargetStatusChanged += OnStatusChanged;
    }

    void OnStatusChanged(ObserverBehaviour behaviour, TargetStatus targetStatus)
    {
        Debug.Log("im updated");
        if (targetStatus.Status == Status.TRACKED || targetStatus.Status == Status.EXTENDED_TRACKED)
        {
            Debug.Log("Playing audio");
            audioSource.Play();
        }
        else
        {
            Debug.Log("Stopping audio");
            audioSource.Stop();
        }
    }
}
