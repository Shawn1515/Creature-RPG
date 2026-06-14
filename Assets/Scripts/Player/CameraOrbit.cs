using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform player;
    public float mouseSensitivity = 120f;
    private float verticalRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if(GameManager.Instance.CurrentState == GameState.Battle || GameManager.Instance.CurrentState == GameState.Party)
        {
            return;
        }
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        player.Rotate(Vector3.up * mouseX);
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -20f, 45f);

        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}