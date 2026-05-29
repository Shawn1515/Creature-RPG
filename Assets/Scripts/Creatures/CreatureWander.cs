using UnityEngine;

public class CreatureWander : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 5f;

    [Header("Wander Settings")]
    public float wanderRadius = 5f;
    public float waitTime = 2f;
    private Vector3 spawnPosition;
    private Vector3 targetPosition;

    private float waitTimer;
    private bool isWaiting;

    void Start()
    {
        spawnPosition = transform.position;
        PickNewTarget();
    }

    void Update()
    {
        if (isWaiting)
        {
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

    void MoveToTarget()
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;
        if (direction.magnitude < 0.2f) {
            StartWaiting();
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    void PickNewTarget() {
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        targetPosition = spawnPosition + new Vector3(randomCircle.x, 0f, randomCircle.y);
    }

    void StartWaiting()
    {
        isWaiting = true;
        waitTimer = waitTime;
    }
}