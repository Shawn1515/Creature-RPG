
using UnityEngine;

public class LastSafePosition : MonoBehaviour
{
    public static LastSafePosition Instance;

    [Header("Safe Position Settings")]
    public float groundCheckDistance = 0.8f;
    public float groundCheckHeight = 0.3f;
    public float groundCheckDepth = 1.5f;
    public LayerMask groundLayer;

    private CharacterController characterController;
    private Vector3 lastSafePosition;

    private void Awake()
    {
        Instance = this;
        characterController = GetComponent<CharacterController>();

        lastSafePosition = transform.position;

        // Use the Ground layer by default if it exists.
        if (groundLayer.value == 0)
            groundLayer = LayerMask.GetMask("Ground");
    }

    private void Update()
    {
        if (characterController == null ||
            !characterController.isGrounded)
            return;

        // Find the direction the player is actually moving.
        Vector3 direction = characterController.velocity;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
            direction.Normalize();
        else
            direction = transform.forward;

        // Check whether ground exists a short distance ahead.
        Vector3 feetPosition = characterController.bounds.min;

        Vector3 checkPosition =
            new Vector3(feetPosition.x, feetPosition.y + groundCheckHeight,
                        feetPosition.z)
            + direction * groundCheckDistance;

        bool groundAhead = Physics.Raycast(
            checkPosition,
            Vector3.down,
            groundCheckDepth,
            groundLayer
        );

        // Only save the position if the player is safely on the ground.
        if (groundAhead)
            lastSafePosition = transform.position;
    }

    public Vector3 GetLastSafePosition()
    {
        return lastSafePosition;
    }
}
