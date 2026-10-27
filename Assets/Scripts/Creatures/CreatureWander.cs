
using UnityEngine;

public class CreatureWander : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 5f;

    [Header("Wander Settings")]
    public float wanderRadius = 5f;
    public float waitTime = 2f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private Vector3 spawnPosition;
    private Vector3 targetPosition;
    private float waitTimer;
    private bool isWaiting;
    private bool canMove = true;

    void Start()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        spawnPosition = transform.position;
        PickNewTarget();
        UpdateAnimation(false);
    }

    void Update()
    {
        if (!canMove ||
            GameManager.Instance.CurrentState != GameState.Exploration)
        {
            UpdateAnimation(false);
            return;
        }

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
            {
                isWaiting = false;
                PickNewTarget();
            }

            UpdateAnimation(false);
            return;
        }

        MoveToTarget();
    }

    void MoveToTarget()
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction.magnitude < 0.2f)
        {
            StartWaiting();
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

        UpdateAnimation(true);
        Debug.DrawLine(transform.position, targetPosition, Color.red);
    }

    void UpdateAnimation(bool isMoving)
    {
        if (animator != null)
            animator.SetBool("IsMoving", isMoving);
    }

    void PickNewTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        targetPosition = spawnPosition +
            new Vector3(randomCircle.x, 0f, randomCircle.y);
    }

    void StartWaiting()
    {
        isWaiting = true;
        waitTimer = waitTime;
    }

    public void StopMoving()
    {
        canMove = false;
        UpdateAnimation(false);
    }

    public void StartMoving()
    {
        canMove = true;
    }
}
