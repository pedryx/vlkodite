using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class FMODFadeBeforeDeactivate : MonoBehaviour
{
    [SerializeField] private StudioEventEmitter musicEmitter;
    [SerializeField] private GameObject objectToDeactivate;
    [SerializeField] private GameObject cameraToDeactivate; // Optional Cinemachine camera
    [SerializeField] private float fadedVolume = 0.2f;
    [SerializeField] private float fadeDuration = 1.0f;

    [Header("Optional Trigger Sound")]
    [SerializeField] private EventReference activationSound;

    private EventInstance musicInstance;
    private bool activationSoundPlayed = false;

    private void Start()
    {
        if (musicEmitter != null)
        {
            musicInstance = musicEmitter.EventInstance;

            if (!musicEmitter.IsPlaying())
                musicEmitter.Play();
        }
        else
        {
            Debug.LogWarning("FMOD emitter not assigned!");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Play activation sound only once
            if (!activationSoundPlayed && !activationSound.IsNull)
            {
                RuntimeManager.PlayOneShot(activationSound, transform.position);
                activationSoundPlayed = true;
            }

            TriggerFadeAndDeactivate();
        }
    }

    public void TriggerFadeAndDeactivate()
    {
        if (musicInstance.isValid())
        {
            StopAllCoroutines();
            StartCoroutine(FadeOutThenDeactivate());
        }
    }

    private IEnumerator FadeOutThenDeactivate()
    {
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

        if (objectToDeactivate != null)
            objectToDeactivate.SetActive(false);

        if (cameraToDeactivate != null)
            cameraToDeactivate.SetActive(false);
    }
}
