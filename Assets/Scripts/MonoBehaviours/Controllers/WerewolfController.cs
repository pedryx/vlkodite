using System;

using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(PathFollow))]
public class WerewolfController : MonoBehaviour
{
    [SerializeField]
    private GameObject werewolfSprite;
    [SerializeField]
    private Transform playerTransform;

    private CharacterMovement characterMovement;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PathFollow pathFollow;

    public event EventHandler OnPlayerCaught;

    private void Awake()
    {
        GameManager.Instance.OnDayBegin += Instance_OnDayBegin;
        GameManager.Instance.OnNightBegin += Instance_OnNightBegin;
        OnPlayerCaught += WerewolfController_OnPlayerCaught;

        characterMovement = GetComponent<CharacterMovement>();
        pathFollow = GetComponent<PathFollow>();

        animator = werewolfSprite.GetComponent<Animator>();
        spriteRenderer = werewolfSprite.GetComponent<SpriteRenderer>();

        enabled = false;
    }

    private void Update()
    {
        // TODO: thi s should happen only when werewolf is moving
        spriteRenderer.flipX = characterMovement.GetFacingDirection() == Direction.Left;
        animator.SetBool("IsMoving", !characterMovement.IsVelocityZero());
    }

    private void OnEnable()
    {
        werewolfSprite.SetActive(true);
        pathFollow.enabled = true;
    }

    private void OnDisable()
    {
        werewolfSprite.SetActive(false);
        pathFollow.enabled = false;
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
