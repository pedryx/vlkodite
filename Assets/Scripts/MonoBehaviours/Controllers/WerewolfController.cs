using System;

using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(PathFollow))]
public class WerewolfController : Singleton<WerewolfController>
{
    [SerializeField]
    private GameObject werewolfSprite;

    private CharacterMovement characterMovement;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PathFollow pathFollow;

    /// <summary>
    /// Occur when player is caught by the werewolf.
    /// </summary>
    public event EventHandler OnPlayerCaught;

    protected override void Awake()
    {
        base.Awake();

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
        if (!characterMovement.IsMoving())
            spriteRenderer.flipX = characterMovement.GetFacingDirection() == Direction.Left;
        animator.SetBool("IsMoving", !characterMovement.IsMoving());
    }

    private void OnEnable()
    {
        werewolfSprite.SetActive(true);
        pathFollow.Target = PlayerController.Instance.gameObject.transform;
    }

    private void OnDisable()
    {
        werewolfSprite.SetActive(false);
        pathFollow.Target = null;
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
