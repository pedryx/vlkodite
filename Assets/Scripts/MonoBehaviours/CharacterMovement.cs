using FMOD.Studio;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    private Rigidbody2D rigidBody;

    private Vector2 movementDirection = Vector2.zero;

	/// <summary>
	/// Threshold below which is movement speed considered as zero.
	/// </summary>
	public const float ZeroSpeedThreshold = 1e-2f;
    /// <summary>
    /// Threshold below which is movement speed squared considered as zero.
    /// </summary>
    public const float ZeroSpeedThresholdSquared = ZeroSpeedThreshold * ZeroSpeedThreshold;

    /// <summary>
    /// Maximum movement speed of the character.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Maximum movement speed of the character.")]
    public float Speed { get; set; } = 1.0f;

    /// <summary>
    /// Current velocity. It is vector of magnitude movement speed per second, pointing in movement direction.
    /// </summary>
    public Vector2 Velocity { get; private set; } = Vector2.zero;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Velocity = Speed * movementDirection;
    }

    private void FixedUpdate()
    {
        rigidBody.position += Velocity * Time.fixedDeltaTime;
    }

    /// <summary>
    /// Move character in a specified direction. When specified direction is <see cref="Vector2.zero"/>, movement is
    /// stopped.
    /// </summary>
    public void Move(Vector2 direction)
    {
        movementDirection = direction.normalized;
    }

    /// <summary>
    /// Move character towards a specified position.
    /// </summary>
    public void MoveTo(Vector2 position)
    {
        movementDirection = (position - (Vector2)transform.localPosition).normalized;
    }

    /// <summary>
    /// Determine if velocity is zero.
    /// </summary>
    public bool IsNotMoving()
    {
        return Velocity.sqrMagnitude < ZeroSpeedThresholdSquared;
    }

    /// <summary>
    /// Get movement direction, prioritizing horizontal direction upon vertical when movement is diagonal.
    /// </summary>
    public Direction GetFacingDirection()
    {
        if (Velocity.x < -ZeroSpeedThreshold)
            return Direction.Left;
        if (Velocity.x > ZeroSpeedThreshold)
            return Direction.Right;
        if (Velocity.y > ZeroSpeedThreshold)
            return Direction.Up;
        if (Velocity.y < -ZeroSpeedThreshold)
            return Direction.Down;

        return Direction.None;
    }
}
