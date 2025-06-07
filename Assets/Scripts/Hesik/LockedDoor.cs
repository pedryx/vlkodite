using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    public string requiredKeyID;
    public Collider2D doorCollider;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && PlayerKeys.Instance.HasKey(requiredKeyID))
        {
            UnlockDoor();
        }
    }

    private void Start()
    {
        PlayerKeys.Instance.OnKeyCollected += HandleKeyCollected;
    }

    private void OnDestroy()
    {
        if (PlayerKeys.Instance != null)
            PlayerKeys.Instance.OnKeyCollected -= HandleKeyCollected;
    }

    void HandleKeyCollected(string collectedKey)
    {
        if (collectedKey == requiredKeyID)
        {
            UnlockDoor();
        }
    }

    void UnlockDoor()
    {
        Debug.Log("Door unlocked with key: " + requiredKeyID);
        doorCollider.enabled = false;
        this.enabled = false; // optional: disable further checks
    }

}
