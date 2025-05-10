using UnityEngine;
using Unity.Cinemachine;

public class CameraZoneConfiner : MonoBehaviour
{
    public Collider2D newConfinerShape;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var vcam = Object.FindFirstObjectByType<CinemachineCamera>();
        if (vcam != null && vcam.TryGetComponent(out CinemachineConfiner2D confiner) && newConfinerShape != null)
        {
            confiner.BoundingShape2D = newConfinerShape;
            confiner.InvalidateBoundingShapeCache(); // Update internal cache
        }
    }
}
