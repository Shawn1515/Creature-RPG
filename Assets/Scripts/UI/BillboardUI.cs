using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Camera mainCamera;
    void Start()
    {
        mainCamera = Camera.main;
    }
    void LateUpdate() {
        Vector3 direction = transform.position - mainCamera.transform.position;
        direction.y = 0f;
        transform.forward = direction;
    }
}