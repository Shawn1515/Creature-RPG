using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FollowerCreature : MonoBehaviour
{
    public Transform player;

    [Header("Follow")]
    public float followDistance = 3f;
    public float moveSpeed = 4f;
    public float rotationSpeed = 10f;

    [Header("Gravity")]
    public float gravity = -20f;

    private CharacterController controller;

    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if(GameManager.Instance.CurrentState == GameState.Exploration)
        {
            FollowPlayer();
            ApplyGravity();
        }
    }

    void FollowPlayer()
    {
        Vector3 playerPosition = player.position;

        Vector3 horizontalOffset =
            new Vector3(
                playerPosition.x - transform.position.x,
                0,
                playerPosition.z - transform.position.z
            );

        float distance = horizontalOffset.magnitude;

        Vector3 movement = Vector3.zero;

        if (distance > followDistance)
        {
            Vector3 direction = horizontalOffset.normalized;

            movement =
                direction *
                moveSpeed;

            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
        }

        controller.Move(
            movement * Time.deltaTime
        );
    }

    void ApplyGravity()
    {
        if (controller.isGrounded &&
            verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity +=
            gravity * Time.deltaTime;

        controller.Move(
            Vector3.up *
            verticalVelocity *
            Time.deltaTime
        );
    }
}