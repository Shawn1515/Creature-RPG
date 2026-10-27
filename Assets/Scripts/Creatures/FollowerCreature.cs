
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NavMeshAgent))]
public class FollowerCreature : MonoBehaviour
{
    public Transform player;

    [Header("Follow")]
    public float followDistance = 3f;
    public float moveSpeed = 4f;
    public float rotationSpeed = 360f;

    [Header("Teleport")]
    public float teleportAfterBlockedSeconds = 5f;
    public float teleportDistance = 15f;
    public float teleportSearchRadius = 3f;

    [Header("Navigation")]
    public float destinationUpdateInterval = 0.2f;

    private NavMeshAgent agent;
    private CharacterController controller;

    private float blockedTimer;
    private float destinationTimer;
    private Animator animator;

    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        controller.enabled = false;

        agent.speed = moveSpeed;
        agent.angularSpeed = rotationSpeed;
        agent.stoppingDistance = followDistance;
        agent.acceleration = 8f;
        agent.autoBraking = true;
    }


    private void Update()
    {
        UpdateAnimation();
        if (GameManager.Instance.CurrentState != GameState.Exploration)
        {
            if (agent.isOnNavMesh)
                agent.ResetPath();

            blockedTimer = 0f;
            return;
        }

        if (player == null || !agent.isOnNavMesh)
            return;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        if (distance <= followDistance)
        {
            agent.ResetPath();
            blockedTimer = 0f;
            return;
        }

        destinationTimer += Time.deltaTime;

        if (destinationTimer >= destinationUpdateInterval)
        {
            agent.SetDestination(player.position);
            destinationTimer = 0f;
        }

        if (!agent.pathPending &&
            (!agent.hasPath ||
             agent.pathStatus != NavMeshPathStatus.PathComplete))
        {
            blockedTimer += Time.deltaTime;
        }
        else
        {
            blockedTimer = 0f;
        }

        if (blockedTimer >= teleportAfterBlockedSeconds ||
            distance >= teleportDistance)
        {
            TryTeleportNearPlayer();
        }
    }

    private void UpdateAnimation()
    {
        if (animator == null)
            return;

        bool isMoving =
            GameManager.Instance.CurrentState == GameState.Exploration &&
            player != null &&
            agent.isOnNavMesh &&
            !agent.pathPending &&
            agent.velocity.sqrMagnitude > 0.01f;

        animator.SetBool("IsMoving", isMoving);
    }

    private void TryTeleportNearPlayer()
    {
        Vector3 targetPosition =
            player.position - player.forward * followDistance;

        if (NavMesh.SamplePosition(
            targetPosition,
            out NavMeshHit hit,
            teleportSearchRadius,
            agent.areaMask))
        {
            TeleportTo(hit.position);
            return;
        }

        if (NavMesh.SamplePosition(
            player.position,
            out hit,
            teleportSearchRadius,
            agent.areaMask))
        {
            TeleportTo(hit.position);
        }
    }

    private void TeleportTo(Vector3 position)
    {
        agent.ResetPath();

        if (agent.Warp(position))
        {
            blockedTimer = 0f;
            destinationTimer = destinationUpdateInterval;
        }
    }
}
