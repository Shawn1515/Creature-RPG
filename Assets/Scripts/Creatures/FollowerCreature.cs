using UnityEngine;

public class FollowerCreature : MonoBehaviour
{
    public Transform player;

    public float followDistance = 3f;
    public float moveSpeed = 4f;
    public float rotateSpeed = 10f;

    private void Update()
    {
        float distance =
            Vector3.Distance(
                transform.position,
                player.position
            );

        if(distance > followDistance)
        {
            Vector3 direction =
                (player.position - transform.position).normalized;

            transform.position +=
                direction *
                moveSpeed *
                Time.deltaTime;

            direction.y = 0;

            if(direction != Vector3.zero)
            {
                Quaternion targetRotation =
                    Quaternion.LookRotation(direction);

                transform.rotation =
                    Quaternion.Slerp(
                        transform.rotation,
                        targetRotation,
                        rotateSpeed * Time.deltaTime
                    );
            }
        }
    }
}