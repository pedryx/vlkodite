using UnityEngine;
using FMODUnity;

public class PlayFMODAndEnableObject : MonoBehaviour
{
    [Header("FMOD")]
    [SerializeField] private EventReference soundEvent;

    [Header("Object to Enable")]
    [SerializeField] private GameObject objectToEnable;

    [Header("Player Tag")]
    [SerializeField] private string playerTag = "Player";

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        if (other.CompareTag(playerTag))
        {
            hasTriggered = true;

            // Play FMOD sound
            if (!soundEvent.IsNull)
            {
                RuntimeManager.PlayOneShot(soundEvent, transform.position);
            }

            // Enable the target GameObject
            if (objectToEnable != null)
            {
                objectToEnable.SetActive(true);
            }
        }
    }
}
