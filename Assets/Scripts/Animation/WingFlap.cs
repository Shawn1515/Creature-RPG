
using UnityEngine;

public class WingFlap : MonoBehaviour
{
    public float flapAngle = 20f;
    public float flapSpeed = 8f;

    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.localRotation;
    }

    void Update()
    {
        float angle = Mathf.Sin(Time.time * flapSpeed) * flapAngle;

        transform.localRotation =
            startRotation * Quaternion.Euler(0f, angle, 0f);
    }
}
