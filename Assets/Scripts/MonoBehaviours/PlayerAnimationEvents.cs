using UnityEngine;
using UnityEngine.Events;

public class PlayerAnimationEvents : MonoBehaviour
{
    [field: SerializeField]
    public UnityEvent OnCookingDone { get; private set; }

    private void Animator_OnCookingDone()
        => OnCookingDone.Invoke();
}
