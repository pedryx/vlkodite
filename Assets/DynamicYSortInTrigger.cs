using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DynamicYSortOnlyWhenInTrigger : MonoBehaviour
{
    [Tooltip("Reference to the player transform")]
    public Transform player;

    [Tooltip("Offset to fine-tune comparison point")]
    public float offset = 0f;

    [Tooltip("Sorting order while outside the trigger zone")]
    public int defaultSortingOrder = 90;

    [Tooltip("Base sorting order when inside the trigger (dynamic offset is applied)")]
    public int baseSortingOrder = 0;

    [Tooltip("Trigger collider that defines the Y-sorting zone")]
    public Collider2D ySortTrigger;

    private SpriteRenderer spriteRenderer;
    private bool isPlayerInTrigger = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = defaultSortingOrder;
    }

    void Update()
    {
        if (player == null || ySortTrigger == null) return;

        bool insideTrigger = ySortTrigger.bounds.Contains(player.position);

        if (insideTrigger)
        {
            isPlayerInTrigger = true;

            float playerY = player.position.y;
            float thisY = transform.position.y + offset;

            // Dynamic sort based on Y comparison
            spriteRenderer.sortingOrder = playerY > thisY
                ? baseSortingOrder + 1
                : baseSortingOrder - 1;
        }
        else if (isPlayerInTrigger)
        {
            // Player exited trigger, reset to default sorting
            isPlayerInTrigger = false;
            spriteRenderer.sortingOrder = defaultSortingOrder;
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector3 gizmoPosition = new Vector3(transform.position.x, transform.position.y + offset, transform.position.z);
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(gizmoPosition, 0.05f);
        Gizmos.DrawLine(transform.position, gizmoPosition);
    }
}
