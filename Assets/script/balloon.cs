using UnityEngine;

public class balloon : MonoBehaviour
{
    public float speed = 1f;
    public float lifetime = 2f;
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }
}
