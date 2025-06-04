using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public string keyID; // e.g., "RedKey", "BlueKey"
    [SerializeField] private GameObject objectToActivate; // Assign your canvas or any GameObject here

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerKeys.Instance.AddKey(keyID);

            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);
            }

            Destroy(gameObject);
        }
    }
}
