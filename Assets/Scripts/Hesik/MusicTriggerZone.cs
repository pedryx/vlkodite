using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class MusicTriggerZone : MonoBehaviour
{
    [SerializeField] private EventReference musicEvent;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private GameObject player;
    [SerializeField] private Collider2D controlZone; //  NEW: reference to the "E to toggle" zone

    private EventInstance musicInstance;
    private bool isPlayerInMusicZone = false;
    private bool isMusicOn = true;
    private float fadeTimer = 0f;
    private float targetVolume = 1f;
    private float currentVolume = 0f;
    private bool isFading = false;

    private void Start()
    {
        musicInstance = RuntimeManager.CreateInstance(musicEvent);
        musicInstance.start();
        musicInstance.setVolume(0f);
        musicInstance.setPaused(true);
    }

    private void Update()
    {
        // Allow toggle only when player is inside controlZone
        if (IsPlayerInControlZone() && Input.GetKeyDown(KeyCode.E))
        {
            ToggleMusic();
        }

        if (isFading)
        {
            fadeTimer += Time.deltaTime;
            float t = Mathf.Clamp01(fadeTimer / fadeDuration);
            float volume = Mathf.Lerp(currentVolume, targetVolume, t);
            musicInstance.setVolume(volume);

            if (t >= 1f)
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
                FadeTo(1f);
                musicInstance.setPaused(false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == player && isPlayerInMusicZone)
        {
            isPlayerInMusicZone = false;
            FadeTo(0f);
            Invoke(nameof(PauseMusic), fadeDuration);
        }
    }

    private bool IsPlayerInControlZone()
    {
        return controlZone != null && controlZone.bounds.Intersects(player.GetComponent<Collider2D>().bounds);
    }

    private void ToggleMusic()
    {
        isMusicOn = !isMusicOn;

        if (isMusicOn)
        {
            if (isPlayerInMusicZone)
            {
                FadeTo(1f);
                musicInstance.setPaused(false);
            }
        }
        else
        {
            FadeTo(0f);
            Invoke(nameof(PauseMusic), fadeDuration);
        }
    }

    private void FadeTo(float volume)
    {
        currentVolume = GetCurrentVolume();
        targetVolume = volume;
        fadeTimer = 0f;
        isFading = true;

        if (volume == 1f)
        {
            CancelInvoke(nameof(PauseMusic));
        }
    }

    private void PauseMusic()
    {
        if (!isPlayerInMusicZone || !isMusicOn)
        {
            musicInstance.setPaused(true);
        }
    }

    private float GetCurrentVolume()
    {
        musicInstance.getVolume(out float vol);
        return vol;
    }

    private void OnDestroy()
    {
        musicInstance.release();
    }
}
