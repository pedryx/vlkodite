using UnityEngine;
using FMODUnity;

public class SFXTrigger : MonoBehaviour
{
    [SerializeField] private EventReference sfxEvent;
    [SerializeField] private Collider2D targetTrigger; // The specific trigger collider

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only respond if THIS specific trigger is entered
        if (other.CompareTag("Player") && other.IsTouching(targetTrigger))
        {
            RuntimeManager.PlayOneShot(sfxEvent, transform.position);
        }
    }
}
