using UnityEngine;
using UnityEngine.Events;

public class AnimationEvent : StateMachineBehaviour
{
    /// <summary>
    /// Event which will occur when animation enters this state.
    /// </summary>
    [field: SerializeField]
    public UnityEvent OnEnter { get; private set; } = new();

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        => OnEnter.Invoke();
}
