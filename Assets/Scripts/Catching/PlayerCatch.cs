using UnityEngine;
using System.Collections;

public class PlayerCatch : MonoBehaviour
{
    public static PlayerCatch Instance;
    public Animator animator;
    public Transform hatSpawnPoint;
    public GameObject catchHatPrefab;
    public Transform playerTransform;
    private Transform target;
    private bool caught;
    
    private GameObject currentHat;

    private void Awake()
    {
        Instance = this;
    }

    public void StartCatch(Transform enemyTarget, bool willCatch)
    {
        StartCoroutine(RotateToTarget(Quaternion.Euler(0f, -90f, 0f) * playerTransform.rotation));        target = enemyTarget;
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
        caught = willCatch;
        
    }

    public void ThrowHat()
    {
        if(currentHat == null)
        {
            return;
        }
        currentHat.transform.SetParent(null);
        currentHat.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        CatchHat hat = currentHat.GetComponent<CatchHat>();
        hat.StartThrow(target, caught);
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