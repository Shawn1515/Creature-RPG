using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float rotationSpeed = 12f;
    public float jumpHeight = 1.5f;
    public float gravity = -25f;
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Exploration)
        {
            return;
        }
        float moveSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(h, 0, v).normalized;

        Vector3 move = Vector3.zero;

        if (input.magnitude >= 0.1f)
        {
            float targetAngle =
                Mathf.Atan2(input.x, input.z) * Mathf.Rad2Deg +
                cameraTransform.eulerAngles.y;

            Vector3 direction =
                Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            move = direction.normalized * moveSpeed;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMove = move * Time.deltaTime + velocity * Time.deltaTime;
        controller.Move(finalMove);
    }
}