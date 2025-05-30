using UnityEngine;
using FMODUnity;

public class PlayFMODOnEInTrigger : MonoBehaviour
{
    [SerializeField] private EventReference soundEvent;
    [SerializeField] private string playerTag = "Player";

    private bool isPlayerInside = false;
    private bool hasPlayed = false;

    private void Update()
    {
        if (isPlayerInside && !hasPlayed && Input.GetKeyDown(KeyCode.E))
        {
            if (!soundEvent.IsNull)
            {
                RuntimeManager.PlayOneShot(soundEvent, transform.position);
                hasPlayed = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInside = false;
        }
    }
}
