using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class PlayerTrigger : MonoBehaviour
{
    private Collider2D trigger;

    [field: SerializeField]
    public UnityEvent OnEnter { get; private set; } = new();
    [field: SerializeField]
    public UnityEvent OnExit { get; private set; } = new();

    private void Awake()
    {
        trigger = GetComponent<Collider2D>();
        Debug.Assert(trigger.isTrigger, "Associated collider should be a trigger.");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!enabled || collision.GetComponent<PlayerController>() == null)
            return;

        OnEnter.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!enabled || collision.GetComponent<PlayerController>() == null)
            return;

        OnExit.Invoke();
    }
}
