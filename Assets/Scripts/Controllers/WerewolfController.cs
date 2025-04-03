using System;

using UnityEngine;

// TODO: make disabled on day

[RequireComponent(typeof(Rigidbody2D))]
public class WerewolfController : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform;

    private Vector2 velocity;
    private Rigidbody2D rigidBody;

    [field: SerializeField]
    public float MovementSpeed { get; private set; } = 1.5f;

    public event EventHandler OnPlayerCaught;

    private void Instance_OnDayBegin(object sender, EventArgs e)
    {
        rigidBody.linearVelocity = Vector2.zero;
    }

    private void Instance_OnNightBegin(object sender, EventArgs e)
    {
        // TODO: change sprite
        GetComponentInChildren<SpriteRenderer>().color = Color.black;
        Debug.Log("were transformed");
    }

    private void Awake()
    {
        // TODO: pre-compute path-finding
        GameManager.Instance.OnDayBegin += Instance_OnDayBegin;
        GameManager.Instance.OnNightBegin += Instance_OnNightBegin;

        rigidBody = GetComponent<Rigidbody2D>();

        OnPlayerCaught += WerewolfController_OnPlayerCaught;
    }

    private void WerewolfController_OnPlayerCaught(object sender, EventArgs e)
    {
        Debug.Log("Player caught");
    }

    private void Update()
    {
        if (GameManager.Instance.IsDay)
            return;

        Vector2 movementDirection = playerTransform.position - transform.position;
        velocity = movementDirection.normalized * MovementSpeed;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.IsDay)
            return;

        rigidBody.position += velocity;
        Debug.Log(velocity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.Instance.IsDay)
            return;
        if (collision.GetComponent<PlayerController>() == null)
            return;

        OnPlayerCaught?.Invoke(this, new EventArgs());
    }
}
