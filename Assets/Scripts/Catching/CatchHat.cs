using UnityEngine;
using System.Collections;
using System;

public class CatchHat : MonoBehaviour
{
    public GameObject catchSuccessParticlesPrefab;
    public GameObject breakoutParticlesPrefab;

    public float rotationSpeed = 720f;

    public float shrinkDuration = 0.4f;

    public int shakeCount = 3;
    public float shakeAngle = 20f;
    public float shakeDuration = 0.2f;

    private Transform target;
    private Vector3 startPosition;
    private Vector3 originalScale;
    private float timer;
    private float arcHeight;
    private float maxScaleMultiplier;

    private float flightDuration;

    private bool caught;

    private bool isTrainerSendOut;
    private System.Action onSendOutComplete;

    public void StartThrow(Transform newTarget, bool willCatch)
    {
        target = newTarget;

        caught = willCatch;

        startPosition = transform.position;

        timer = 0f;

        arcHeight = 2f;

        flightDuration = 1.5f;

        maxScaleMultiplier = 25;
    }

    public void StartSendOut(Transform newTarget, System.Action onComplete)
    {
        target = newTarget;
        caught = true;
        isTrainerSendOut = true;
        onSendOutComplete = onComplete;
        startPosition = transform.position;
        timer = 0f;
        arcHeight = 1f;
        flightDuration = 0.75f;
        maxScaleMultiplier = 10;
    }

    void Awake()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (target == null)
        {
            return;
        }

        timer += Time.deltaTime;

        float progress = timer / flightDuration;
        if(progress > flightDuration / 1.5f)
        {
            target.gameObject.SetActive(false);
        }
        float expansionProgress = Mathf.InverseLerp(
            0.4f,
            1f,
            progress
        );

        transform.localScale = Vector3.Lerp(
            originalScale,
            originalScale * maxScaleMultiplier,
            expansionProgress
        );
        Vector3 position = Vector3.Lerp(
            startPosition,
            target.position,
            progress
        );
        transform.Rotate(
            Vector3.up,
            rotationSpeed * Time.deltaTime
        );

        position.y += Mathf.Sin(progress * Mathf.PI) * arcHeight;
        transform.position = position;

        if (progress >= 1f)
        {
            StartCoroutine(ReachCreature());
        }
    }

    IEnumerator ReachCreature()
    {
        target = null;

        if (isTrainerSendOut)
        {
            PlayBreakoutParticles();
            onSendOutComplete?.Invoke();
            yield break;
        }

        yield return StartCoroutine(ShrinkHat());
        yield return StartCoroutine(ShakeHat());
        if(!caught)
        {
            StartCoroutine(BreakHat());
        }
        else
        {
            PlayCatchSuccessParticles();
        }
        StartCoroutine(BattleManager.Instance.CatchAfterAnimation(caught));
    }

    IEnumerator ShrinkHat()
    {
        Vector3 startScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / shrinkDuration;
            transform.localScale = Vector3.Lerp(
                startScale,
                originalScale * 3f,
                progress
            );
            yield return null;
        }
        transform.localScale = originalScale * 3f;
    }

    IEnumerator BreakHat()
    {
        PlayBreakoutParticles();
        Vector3 startScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < 0.2f)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / 0.2f;
            transform.localScale = Vector3.Lerp(
                startScale,
                originalScale * maxScaleMultiplier,
                progress
            );
            yield return null;
        }
        transform.localScale = originalScale * maxScaleMultiplier;
    }

    IEnumerator ShakeHat()
    {
        Quaternion originalRotation = transform.rotation;

        for (int i = 0; i < shakeCount; i++)
        {
            Quaternion leftRotation =
                originalRotation *
                Quaternion.Euler(shakeAngle, 0f, 0f);

            Quaternion rightRotation =
                originalRotation *
                Quaternion.Euler(-shakeAngle, 0f, 0f);

            float elapsed = 0f;

            while (elapsed < shakeDuration)
            {
                elapsed += Time.deltaTime;

                transform.rotation = Quaternion.Lerp(
                    originalRotation,
                    leftRotation,
                    elapsed / shakeDuration
                );

                yield return null;
            }

            elapsed = 0f;

            while (elapsed < shakeDuration) {
                elapsed += Time.deltaTime;

                transform.rotation = Quaternion.Lerp(
                    leftRotation,
                    rightRotation,
                    elapsed / shakeDuration
                );

                yield return null;
            }

            elapsed = 0f;

            while (elapsed < shakeDuration)
            {
                elapsed += Time.deltaTime;

                transform.rotation = Quaternion.Lerp(
                    rightRotation,
                    originalRotation,
                    elapsed / shakeDuration
                );

                yield return null;
            }
        }

        transform.rotation = originalRotation;
    }

    void PlayCatchSuccessParticles()
    {
        Instantiate(
            catchSuccessParticlesPrefab,
            transform.position,
            Quaternion.identity
        );
    }

    void PlayBreakoutParticles()
    {
        Instantiate(
            breakoutParticlesPrefab,
            transform.position,
            Quaternion.identity
        );
    }
}