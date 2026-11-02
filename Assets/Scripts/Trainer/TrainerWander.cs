
using UnityEngine;

public class TrainerWander : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 5f;

    [Header("Wander Settings")]
    public float wanderRadius = 5f;
    public float waitTime = 2f;

    [Header("Detection")]
    public float detectionRange = 6f;
    public float fieldOfView = 120f;

    [Header("Grounding")]
    public LayerMask groundLayer;
    public float groundCheckHeight = 2f;
    public float groundCheckDistance = 5f;
    

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private Vector3 spawnPosition;
    private Vector3 targetPosition;
    private float waitTimer;
    private bool isWaiting;
    private bool hasSpottedPlayer;

    private Transform player;
    private TrainerEncounter trainerEncounter;
    

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        spawnPosition = transform.position;

        trainerEncounter = GetComponent<TrainerEncounter>();

        player = GameObject.FindGameObjectWithTag("Player").transform;

        PickNewTarget();
    }

    void Update()
    {
        if(trainerEncounter.Defeated())
        {
            UpdateAnimation(false);
            TrainerWander wander = GetComponent<TrainerWander>();
            if (wander != null)
            {
                wander.enabled = false;
            }
        }

        if (GameManager.Instance.CurrentState != GameState.Exploration)
        {
            UpdateAnimation(false);
            return;
        }

        if (hasSpottedPlayer)
        {
            UpdateAnimation(false);
            return;
        }

        if (CanSeePlayer())
        {
            UpdateAnimation(false);
            ChallengePlayer();
            return;
        }

        if (isWaiting)
        {
            UpdateAnimation(false);
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
            {
                isWaiting = false;
                PickNewTarget();
            }

            return;
        }

        MoveToTarget();
    }

    void ApplyGrounding()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * groundCheckHeight;

        if (Physics.Raycast(
            rayOrigin,
            Vector3.down,
            out RaycastHit hit,
            groundCheckDistance,
            groundLayer))
        {
            Vector3 position = transform.position;
            position.y = hit.point.y + trainerEncounter.trainer.groundOffset;
            transform.position = position;
        }
    }

    void UpdateAnimation(bool isMoving)
    {
        if (animator != null)
            animator.SetBool("IsMoving", isMoving);
    }

    void MoveToTarget()
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction.magnitude < 0.2f)
        {
            isWaiting = true;
            waitTimer = waitTime;
            UpdateAnimation(false);
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        transform.position += direction.normalized * moveSpeed * Time.deltaTime;
        ApplyGrounding();
        UpdateAnimation(true);
    }

    bool CanSeePlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.magnitude > detectionRange)
            return false;

        float angle = Vector3.Angle(transform.forward, direction);

        return angle <= fieldOfView / 2f;
    }

    void ChallengePlayer()
    {
        hasSpottedPlayer = true;

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        trainerEncounter.Interact();
    }

    void PickNewTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;

        targetPosition = spawnPosition +
            new Vector3(randomCircle.x, 0f, randomCircle.y);
    }
}
