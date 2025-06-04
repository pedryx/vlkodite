using UnityEngine;

public class EnableOnTrigger : MonoBehaviour
{
    [SerializeField] private GameObject objectToEnable;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && objectToEnable != null)
        {
            objectToEnable.SetActive(true);
        }
    }
}
