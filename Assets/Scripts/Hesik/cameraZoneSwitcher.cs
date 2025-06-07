using UnityEngine;
using Unity.Cinemachine;

public class CameraZoneSwitcher : MonoBehaviour
{
    public CinemachineCamera targetCamera;
    public const int activePriority = 20;
    public const int defaultPriority = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var allCams = Object.FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None);
        foreach (var cam in allCams)
            cam.Priority = defaultPriority;

        if (targetCamera != null)
            targetCamera.Priority = activePriority;
    }
}
