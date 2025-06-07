using UnityEngine;
using FMODUnity;

public class PlayOnceSound : MonoBehaviour
{
    [SerializeField] private EventReference soundEvent;

    private bool hasPlayed = false;

    void OnEnable()
    {
        if (!hasPlayed)
        {
            RuntimeManager.PlayOneShot(soundEvent, transform.position);
            hasPlayed = true;

            // Disable this component so it won't play again
            this.enabled = false;
        }
    }
}
