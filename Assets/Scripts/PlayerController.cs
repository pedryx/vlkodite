using UnityEngine;

/// <summary>
/// Script responsible for controlling player's movement.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private Vector2 targetVelocity;

    /// <summary>
    /// Time in second which it takes player to reach <see cref="MovementSpeed"/>.
    /// 
    /// When player starts moving, there is a small acceleration window. This determines duration (in seconds) of that window.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Time in second which it takes player to reach its maximum movement speed.\n\n" +
        "When player starts moving, there is a small acceleration window. This determines duration (in seconds) of that window.")]
    public float MovementStartTime { get; private set; } = 0.3f;
    /// <summary>
    /// Time in second which it takes player to stop moving.
    /// 
    /// When player stops moving, there is a small de-acceleration window. This determines duration (in seconds) of that window.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Time in second which it takes player to stop moving.\n\n" +
        "When player stops moving, there is a small de-acceleration window. This determines duration (in seconds) of that window.")]
    public float MovementEndTime { get; private set; } = 0.1f;
    /// <summary>
    /// Maximum movement speed of player in pixels per second.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Maximum movement speed of player in pixels per second.")]
    public float MovementSpeed { get; private set; } = 1.0f;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Vector2 inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        targetVelocity = inputDirection * MovementSpeed;
    }

    private void FixedUpdate()
    {
        float rate = (targetVelocity.magnitude > 0 ? MovementSpeed / MovementStartTime : MovementSpeed / MovementEndTime);

        Vector2 force = Vector2.ClampMagnitude(targetVelocity - rigidBody.linearVelocity, rate * Time.fixedDeltaTime);
        rigidBody.AddForce(force, ForceMode2D.Impulse);
    }
}
