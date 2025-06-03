using UnityEngine;
using UnityEngine.Events;

public class WerewolfAnimationEvents : MonoBehaviour
{
    public UnityEvent InCatchFrame { get; private set; } = new();
    public UnityEvent AfterLastFrame { get; private set; } = new();

    private void Animator_InCatchFrame()
    {
        InCatchFrame.Invoke();
    }

    private void Animator_AfterLastFrame()
    {
        AfterLastFrame.Invoke();
    }
}
