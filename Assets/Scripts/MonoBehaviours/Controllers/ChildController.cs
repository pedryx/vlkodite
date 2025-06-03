using UnityEngine;

public class ChildController : Singleton<ChildController>
{
    [SerializeField] private GameObject humanForm;
    [SerializeField] private Animator animator;

    [Header("Unity 6 Cinemachine Camera (Optional)")]
    [SerializeField] private MonoBehaviour cameraToDisableWhileMoving; // Use actual type if known

    private CharacterMovement movement;
    private Vector3 spawnPosition;

    private void Awake()
    {
        movement = GetComponent<CharacterMovement>();
        spawnPosition = transform.position;

        GameManager.Instance.OnDayBegin.AddListener(GameManager_OnDayBegin);
        GameManager.Instance.OnNightBegin.AddListener(GameManager_OnNightBegin);
    }

    private void OnEnable()
    {
        humanForm.SetActive(true);
    }

    private void OnDisable()
    {
        humanForm.SetActive(false);
    }

    private void GameManager_OnDayBegin()
    {
        transform.position = spawnPosition;
        enabled = true;
    }

    private void GameManager_OnNightBegin()
    {
        enabled = false;
    }

    private void Update()
    {
        bool isMoving = !movement.IsNotMoving();
        animator.SetBool("isMoving", isMoving);

        // Flip child based on X movement
        float x = movement.Velocity.x;
        if (Mathf.Abs(x) > CharacterMovement.ZeroSpeedThreshold)
        {
            Vector3 scale = transform.localScale;
            scale.x = x > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        // Disable Unity 6 Cinemachine camera while moving
        if (cameraToDisableWhileMoving != null)
        {
            cameraToDisableWhileMoving.enabled = !isMoving;
        }
    }
}
