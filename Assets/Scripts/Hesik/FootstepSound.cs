using UnityEngine;
using FMODUnity;

[RequireComponent(typeof(CharacterMovement))]
public class FootstepSound : MonoBehaviour
{
    [SerializeField] private EventReference footstepEvent;

    private CharacterMovement characterMovement;
    private StudioEventEmitter emitter;

    private void Awake()
    {
        characterMovement = GetComponent<CharacterMovement>();
        emitter = gameObject.AddComponent<StudioEventEmitter>();
        emitter.EventReference = footstepEvent;
        emitter.AllowFadeout = true;
        emitter.StopEvent = EmitterGameEvent.None;
        emitter.PlayEvent = EmitterGameEvent.None;
    }

    private void Update()
    {
        bool shouldPlay = !characterMovement.IsNotMoving();

        if (shouldPlay && !emitter.IsPlaying())
        {
            emitter.Play();
        }
        else if (!shouldPlay && emitter.IsPlaying())
        {
            emitter.Stop();
        }
    }
}
