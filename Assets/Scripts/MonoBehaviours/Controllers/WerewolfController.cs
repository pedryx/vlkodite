using System;

using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
public class WerewolfController : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform;

    private CharacterMovement characterMovement;

    public event EventHandler OnPlayerCaught;

    private void Awake()
    {
        GameManager.Instance.OnDayBegin += Instance_OnDayBegin;
        GameManager.Instance.OnNightBegin += Instance_OnNightBegin;
        OnPlayerCaught += WerewolfController_OnPlayerCaught;

        characterMovement = GetComponent<CharacterMovement>();
        enabled = false;
    }

    private void Update()
    {
        characterMovement.Move(playerTransform.position - transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!enabled || !collision.TryGetComponent<PlayerController>(out _))
            return;

        OnPlayerCaught?.Invoke(this, new EventArgs());
    }

    private void WerewolfController_OnPlayerCaught(object sender, EventArgs e)
    {
        Debug.Log("Player caught");
    }

    private void Instance_OnDayBegin(object sender, EventArgs e)
    {
        characterMovement.Move(Vector2.zero);
        enabled = false;
    }

    private void Instance_OnNightBegin(object sender, EventArgs e)
    {
        enabled = true;
    }
}
