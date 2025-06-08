using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class MusicTriggerZone : MonoBehaviour
{
    [SerializeField] private EventReference musicEvent;               // New music to play
    [SerializeField] private StudioEventEmitter emitterToFadeOut;     // Existing music to fade
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private GameObject player;
    [SerializeField] private Collider2D controlZone;

    private EventInstance musicAInstance;
    private bool isPlayerInMusicZone = false;
    private bool isMusicOn = true;

    private float fadeTimerA = 0f;
    private float fadeTimerB = 0f;
    private float targetVolumeA = 1f;
    private float currentVolumeA = 0f;
    private float targetVolumeB = 0f;
    private float currentVolumeB = 1f;

    private bool isFading = false;

    private void Start()
    {
        musicAInstance = RuntimeManager.CreateInstance(musicEvent);
        musicAInstance.setVolume(0f);
        musicAInstance.start();  // Start silent
    }

    private void Update()
    {
        if (IsPlayerInControlZone() && Input.GetKeyDown(KeyCode.E))
        {
            ToggleMusic();
        }

        if (isFading)
        {
            fadeTimerA += Time.deltaTime;
            fadeTimerB += Time.deltaTime;

            float tA = Mathf.Clamp01(fadeTimerA / fadeDuration);
            float tB = Mathf.Clamp01(fadeTimerB / fadeDuration);

            float volumeA = Mathf.Lerp(currentVolumeA, targetVolumeA, tA);
            float volumeB = Mathf.Lerp(currentVolumeB, targetVolumeB, tB);

            musicAInstance.setVolume(volumeA);
            if (emitterToFadeOut != null)
                emitterToFadeOut.EventInstance.setVolume(volumeB);

            if (tA >= 1f && tB >= 1f)
                isFading = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player && !isPlayerInMusicZone)
        {
            isPlayerInMusicZone = true;

            if (isMusicOn)
            {
                FadeTo(1f, 0f); // Fade in A, fade out B
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == player && isPlayerInMusicZone)
        {
            isPlayerInMusicZone = false;

            FadeTo(0f, 1f); // Fade out A, fade in B
        }
    }

    private void ToggleMusic()
    {
        isMusicOn = !isMusicOn;

        if (isMusicOn)
        {
            if (isPlayerInMusicZone)
            {
                FadeTo(1f, 0f);
            }
        }
        else
        {
            FadeTo(0f, 1f);
        }
    }

    private void FadeTo(float volumeA, float volumeB)
    {
        musicAInstance.getVolume(out currentVolumeA);
        if (emitterToFadeOut != null)
            emitterToFadeOut.EventInstance.getVolume(out currentVolumeB);

        targetVolumeA = volumeA;
        targetVolumeB = volumeB;

        fadeTimerA = 0f;
        fadeTimerB = 0f;
        isFading = true;
    }

    private bool IsPlayerInControlZone()
    {
        return controlZone != null && controlZone.bounds.Intersects(player.GetComponent<Collider2D>().bounds);
    }

    private void OnDestroy()
    {
        musicAInstance.release();
    }
}
