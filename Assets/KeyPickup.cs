using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public string keyID; // e.g., "RedKey", "BlueKey"

    [SerializeField] private GameObject objectToActivate;    // Assign in Inspector
    [SerializeField] private GameObject objectToDeactivate;  // Assign in Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerKeys.Instance.AddKey(keyID);

            if (objectToActivate != null)
                objectToActivate.SetActive(true);

            if (objectToDeactivate != null)
                objectToDeactivate.SetActive(false);

            Destroy(gameObject); // Remove the key pickup object
        }
    }
}
