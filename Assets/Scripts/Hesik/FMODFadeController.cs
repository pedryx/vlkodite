using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FMODFadeController : MonoBehaviour
{
    [SerializeField] private StudioEventEmitter musicEmitter; // Assign the object with the music
    [SerializeField] private float fadeOutVolume = 0.2f;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private float holdDuration = 3.0f;

    private EventInstance musicInstance;

    private void Start()
    {
        if (musicEmitter != null)
        {
            musicInstance = musicEmitter.EventInstance;
        }
    }

    private void OnEnable()
    {
        if (musicInstance.isValid())
        {
            StopAllCoroutines();
            StartCoroutine(FadeSequence());
        }
    }

    private System.Collections.IEnumerator FadeSequence()
    {
        yield return StartCoroutine(FadeTo(fadeOutVolume, fadeDuration));
        yield return new WaitForSeconds(holdDuration);
        yield return StartCoroutine(FadeTo(1.0f, fadeDuration));
    }

    private System.Collections.IEnumerator FadeTo(float targetVolume, float duration)
    {
        musicInstance.getVolume(out float startVolume);
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float current = Mathf.Lerp(startVolume, targetVolume, t / duration);
            musicInstance.setVolume(current);
            yield return null;
        }
        musicInstance.setVolume(targetVolume);
    }
}
