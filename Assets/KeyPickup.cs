using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public string keyID; // e.g., "RedKey", "BlueKey"

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerKeys.Instance.AddKey(keyID);
            Destroy(gameObject);
        }
    }
}
