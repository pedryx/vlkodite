using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DynamicYSort : MonoBehaviour
{
    [Tooltip("Reference to the player transform")]
    public Transform player;

    [Tooltip("Optional offset for fine-tuning the sort position")]
    public float offset = 0f;

    [Tooltip("The base sorting order (set this so you can control layers)")]
    public int baseSortingOrder = 0;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (player == null) return;

        float playerY = player.position.y;
        float thisY = transform.position.y + offset;

        // If the player is behind this object (higher Y), render this object in front
        if (playerY > thisY)
        {
            spriteRenderer.sortingOrder = baseSortingOrder + 1;
        }
        else
        {
            spriteRenderer.sortingOrder = baseSortingOrder - 1;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Show where the comparison point is
        Vector3 gizmoPosition = new Vector3(transform.position.x, transform.position.y + offset, transform.position.z);
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(gizmoPosition, 0.05f);
        Gizmos.DrawLine(transform.position, gizmoPosition);
    }
}
