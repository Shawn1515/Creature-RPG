using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    public static ScreenFade Instance;

    public Image fadeImage;

    public GameObject panel;

    private Color color;

    private bool fadeFrom;

    private void Awake()
    {
        Instance = this;
        fadeFrom = false;
        panel.SetActive(false);
        color = fadeImage.color;
    }

    public void FadeToBlack(float duration, Action onComplete = null)
    {
        fadeImage.color = color;
        panel.SetActive(true);
        StartCoroutine(Fade(1f, duration, onComplete));
    }

    public void FadeFromBlack(float duration, Action onComplete = null)
    {
        fadeFrom = true;
        StartCoroutine(Fade(0f, duration, onComplete));
    }

    private IEnumerator Fade(
        float targetAlpha,
        float duration,
        Action onComplete)
    {
        float startAlpha = fadeImage.color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                time / duration
            );

            Color color = fadeImage.color;
            color.a = alpha;
            fadeImage.color = color;

            yield return null;
        }

        Color finalColor = fadeImage.color;
        finalColor.a = targetAlpha;
        fadeImage.color = finalColor;

        if(fadeFrom)
        {
            fadeFrom = false;
            panel.SetActive(false);
        }
        else
        {
            PartyManager.Instance.HealParty();
            BattleManager.Instance.TrainerPositionReset();
            Destroy(FollowerManager.Instance.currentFollower);
            FollowerManager.Instance.currentFollower = null;
            GameManager.Instance.SetState(GameState.Dialogue);
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = Vector3.zero;
            FollowerManager.Instance.SpawnFollower();
            yield return new WaitForSeconds(1f);
        }

        onComplete?.Invoke();
    }

    public void BattleTransition(System.Action onWhite)
    {
        panel.SetActive(true);

        if (battleTransitionCoroutine != null)
            StopCoroutine(battleTransitionCoroutine);

        battleTransitionCoroutine =
            StartCoroutine(BattleFlash(onWhite));
    }

    private Coroutine battleTransitionCoroutine;

    private IEnumerator BattleFlash(System.Action onWhite)
    {
        fadeImage.color = new Color(1f, 1f, 1f, 0f);

        yield return FadeToAlpha(1f, 0.25f);

        onWhite?.Invoke();
        yield return new WaitForSeconds(0.15f);

        yield return FadeToAlpha(0f, 0.5f);

        panel.SetActive(false);
        battleTransitionCoroutine = null;
    }

    private IEnumerator FadeToAlpha(float targetAlpha, float duration)
    {
        float startAlpha = fadeImage.color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            Color color = fadeImage.color;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            fadeImage.color = color;

            yield return null;
        }

        Color finalColor = fadeImage.color;
        finalColor.a = targetAlpha;
        fadeImage.color = finalColor;
    }
}