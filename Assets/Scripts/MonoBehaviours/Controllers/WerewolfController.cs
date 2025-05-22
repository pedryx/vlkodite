using UnityEngine;
using UnityEngine.Events;

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
    private CircleCollider2D circleCollider;

    /// <summary>
    /// Occur when player is caught by the werewolf.
    /// </summary>
    [Tooltip("Occur when player is caught by the werewolf.")]
    public UnityEvent OnPlayerCaught = new();

    protected override void Awake()
    {
        base.Awake();

        GameManager.Instance.OnDayBegin.AddListener(Instance_OnDayBegin);
        GameManager.Instance.OnNightBegin.AddListener(Instance_OnNightBegin);
        OnPlayerCaught.AddListener(WerewolfController_OnPlayerCaught);

        characterMovement = GetComponent<CharacterMovement>();
        pathFollow = GetComponent<PathFollow>();
        circleCollider = GetComponent<CircleCollider2D>();

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
        circleCollider.enabled = true;
    }

    private void OnDisable()
    {
        werewolfSprite.SetActive(false);
        circleCollider .enabled = false;
        pathFollow.Target = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!enabled || !collision.TryGetComponent(out PlayerController player) || player.GodModeActive)
            return;

        OnPlayerCaught.Invoke();
    }

    private void WerewolfController_OnPlayerCaught()
    {
        Debug.Log("Player caught");
    }

    private void Instance_OnDayBegin()
    {
        characterMovement.Move(Vector2.zero);
        enabled = false;
    }

    private void Instance_OnNightBegin()
    {
        enabled = true;
    }
}
