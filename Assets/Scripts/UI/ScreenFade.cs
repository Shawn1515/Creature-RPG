using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    public static ScreenFade Instance;

    public Image fadeImage;

    public GameObject panel;

    private bool fadeFrom;

    private void Awake()
    {
        Instance = this;
        fadeFrom = false;
        panel.SetActive(false);
    }

    public void FadeToBlack(float duration, Action onComplete = null)
    {
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
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = Vector3.zero;
            yield return new WaitForSeconds(1f);
        }

        onComplete?.Invoke();
    }
}