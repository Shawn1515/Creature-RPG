using UnityEngine;

public class BattleCameraTargetRotation : MonoBehaviour
{
    public void FaceBattle(Transform playerCreature, Transform enemy)
    {
        Vector3 midpoint = (playerCreature.position + enemy.position) / 2f;

        Vector3 direction = midpoint - transform.position;

        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}