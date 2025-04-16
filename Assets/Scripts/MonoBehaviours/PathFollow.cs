using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
public class PathFollow : MonoBehaviour
{
    /// <summary>
    /// How often (in seconds) to refresh the path
    /// </summary>
    private const float refreshPeriod = 1.0f;

    private CharacterMovement characterMovement;

    /// <summary>
    /// Tracks time elapsed since the last path update.
    /// </summary>
    private float elapsed = refreshPeriod; // initialized to allow immediate update on start
    /// <summary>
    /// The last known target position used to compute a path. Helps to decide if a new path is necessary.
    /// </summary>
    private Vector2 currentTargetPosition = Vector2.one * float.MaxValue;
    /// <summary>
    /// The current computed path as a series of positions.
    /// </summary>
    private IReadOnlyList<Vector2> path;
    /// <summary>
    /// Index to the next position in the path the character should move towards.
    /// </summary>
    private int pathIndex;

    /// <summary>
    /// The target transform that the character is moving towards.
    /// </summary>
    [field: SerializeField]
    [Tooltip("The target transform that the character is moving towards.")]
    public Transform Target { get; set; }

    private void Awake()
    {
        characterMovement = GetComponent<CharacterMovement>();
    }

    private void Update()
    {
        if (Target == null || (Target.transform.localPosition - transform.localPosition).IsZero())
        {
            characterMovement.Move(Vector2.zero);
            return;
        }

        if (IsTargetVisible())
        {
            elapsed = 0.0f;
            characterMovement.MoveTo(Target.localPosition);
            return;
        }
        if (path == null || pathIndex == path.Count || path.Count == 0)
        {
            elapsed = 0.0f;
            FindNewPathAsync();
            return;
        }

        elapsed += Time.deltaTime;
        if (elapsed >= refreshPeriod)
        {
            elapsed = 0.0f;
            FindNewPathAsync();
        }

        while (pathIndex < path.Count && (path[pathIndex] - (Vector2)transform.localPosition).sqrMagnitude < 1e-2)
            pathIndex++;
        if (pathIndex < path.Count)
            characterMovement.MoveTo(path[pathIndex]);
    }

    private void OnDrawGizmos()
    {
        if (!GameManager.Instance.ShowPathFindingPatches)
            return;
        if (path == null || Target == null || pathIndex == path.Count || path.Count == 0)
            return;

        Gizmos.color = Color.magenta;
        for (int i = 0; i < path.Count - 1; i++)
        {
            Gizmos.DrawLine(path[i].Extend(-2.0f), path[i + 1].Extend(-2.0f));
        }
    }

    /// <summary>
    /// Determine if there is no obstacle between the game object and the target.
    /// </summary>
    private bool IsTargetVisible()
    {
        RaycastHit2D hit = Physics2D.Linecast(
            transform.localPosition,
            Target.localPosition,
            LayerMask.GetMask("Default")
        );

        return hit.collider == null;
    }

    /// <summary>
    /// Checks if the target has moved since the last path calculation and, if so, computes a new path.
    /// </summary>
    private async void FindNewPathAsync()
    {
        if ((Vector2)Target.localPosition == currentTargetPosition)
            return;

        Vector2 position = transform.localPosition;
        Vector2 targetPosition = Target.localPosition;

        path = await Task.Run(() => GameManager.Instance.PathFinder.FindPath(position, targetPosition));
        pathIndex = 0;
        currentTargetPosition = Target.localPosition;
    }
}
