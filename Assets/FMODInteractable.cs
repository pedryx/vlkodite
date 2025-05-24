using UnityEngine;
using FMODUnity;

public class FMODInteractable : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private EventReference interactSound;

    private bool isPlayerNearby = false;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            RuntimeManager.PlayOneShot(interactSound, transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerNearby = false;
        }
    }
}
