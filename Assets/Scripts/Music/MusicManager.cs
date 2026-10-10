using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioSource overworldSource;
    public AudioSource battleSource;

    public AudioClip overworldMusic;
    public AudioClip battleMusic;

    public float fadeDuration = 1f;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        overworldSource.clip = overworldMusic;
        overworldSource.loop = true;
        overworldSource.Play();
    }

  
    public void StartBattleMusic()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        battleSource.Stop();
        battleSource.time = 0f;
        battleSource.volume = 0f;
        battleSource.Play();

        fadeCoroutine = StartCoroutine(Crossfade(
            overworldSource,
            battleSource
        ));
    }

    public void StartOverworldMusic()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        overworldSource.UnPause();

        fadeCoroutine = StartCoroutine(Crossfade(
            battleSource,
            overworldSource
        ));
    }

    private IEnumerator Crossfade(
        AudioSource fadeOut,
        AudioSource fadeIn)
    {
        float startOutVolume = fadeOut.volume;
        float startInVolume = fadeIn.volume;

        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / fadeDuration);

            fadeOut.volume = Mathf.Lerp(
                startOutVolume, 0f, t);

            fadeIn.volume = Mathf.Lerp(
                startInVolume, 1f, t);

            yield return null;
        }

        fadeOut.volume = 0f;
        fadeIn.volume = 1f;
        fadeOut.Pause();
    }
}