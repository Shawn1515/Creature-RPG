
using UnityEngine;

public class FloatingCreature : MonoBehaviour
{
    public float bounceHeight = 0.15f;
    public float bounceSpeed = 2f;
    public float tiltAngle = 5f;
    public float tiltSpeed = 1.5f;

    private Vector3 startPosition;
    private Quaternion startRotation;

    void Start()
    {
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;
    }

    void Update()
    {
        float bounce = Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;

        transform.localPosition = startPosition + Vector3.up * bounce;

        float tilt = Mathf.Sin(Time.time * tiltSpeed) * tiltAngle;

        transform.localRotation =
            startRotation * Quaternion.Euler(0f, 0f, tilt);
    }
}
