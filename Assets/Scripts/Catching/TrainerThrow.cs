using UnityEngine;
using System.Collections;

public class TrainerThrow : MonoBehaviour
{
    public static TrainerThrow Instance;
    public Animator animator;
    public Transform hatSpawnPoint;
    public GameObject catchHatPrefab;
    public Transform playerTransform;
    private Transform target;
    
    private GameObject currentHat;

    private void Awake()
    {
        Instance = this;
    }

    public void StartThrow(Transform enemyTarget)
    {
        StartCoroutine(RotateToTarget(Quaternion.Euler(0f, -90f, 0f) * playerTransform.rotation));
        target = enemyTarget;
        animator.SetTrigger("Catch");
        if(currentHat != null)
        {
            return;
        }
        currentHat = Instantiate(catchHatPrefab,
            hatSpawnPoint.position,
            catchHatPrefab.transform.rotation,
            hatSpawnPoint
        );
    }

    public void ThrowHat(GameObject currentEnemy)
    {
        if(currentHat == null)
        {
            return;
        }
        currentHat.transform.SetParent(null);
        currentHat.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        CatchHat hat = currentHat.GetComponent<CatchHat>();
        hat.StartThrow(target, currentEnemy);
    }

    public void DestroyHat()
    {
        if(currentHat == null)
        {
            return;
        }
        Destroy(currentHat);
        currentHat = null;
    }

    public void DoneThrowing()
    {
        StartCoroutine(RotateToTarget(Quaternion.Euler(0f, 90f, 0f) * playerTransform.rotation));
    }

    IEnumerator RotateToTarget(Quaternion targetRotation)
    {
        while (Quaternion.Angle(playerTransform.rotation, targetRotation) > 1f)
        {
            playerTransform.rotation = Quaternion.Slerp(
                playerTransform.rotation,
                targetRotation,
                6f * Time.deltaTime
            );
        
            yield return null;
        }
        playerTransform.rotation = targetRotation;
    }

}