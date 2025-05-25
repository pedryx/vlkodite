using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class FMODFadeBeforeDeactivate : MonoBehaviour
{
    [SerializeField] private StudioEventEmitter musicEmitter; // The music emitter to fade
    [SerializeField] private GameObject objectToDeactivate;   // The object you want to deactivate
    [SerializeField] private float fadedVolume = 0.2f;
    [SerializeField] private float fadeDuration = 1.0f;

    private EventInstance musicInstance;

    private void Start()
    {
        if (musicEmitter != null)
        {
            musicInstance = musicEmitter.EventInstance;

            // Optional: force play if not already playing
            if (!musicEmitter.IsPlaying())
            {
                musicEmitter.Play();
            }
        }
        else
        {
            Debug.LogWarning("FMOD emitter not assigned!");
        }
    }

    public void TriggerFadeAndDeactivate()
    {
        if (musicInstance.isValid() && objectToDeactivate != null)
        {
            StopAllCoroutines();
            StartCoroutine(FadeOutThenDeactivate());
        }
    }

    private IEnumerator FadeOutThenDeactivate()
    {
        // Fade out
        musicInstance.getVolume(out float startVolume);
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float current = Mathf.Lerp(startVolume, fadedVolume, t / fadeDuration);
            musicInstance.setVolume(current);
            yield return null;
        }

        musicInstance.setVolume(fadedVolume);

        // Then deactivate the object
        objectToDeactivate.SetActive(false);
    }
}
