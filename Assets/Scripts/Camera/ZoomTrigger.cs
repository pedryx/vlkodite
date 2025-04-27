using UnityEngine;
using Unity.Cinemachine;

public class ZoomTrigger : MonoBehaviour
{
    public CinemachineCamera zoomCamera;
    public CinemachineCamera playerCamera;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            zoomCamera.Priority = 11;
            playerCamera.Priority = 10;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerCamera.Priority = 11;
            zoomCamera.Priority = 10;
        }
    }
}
