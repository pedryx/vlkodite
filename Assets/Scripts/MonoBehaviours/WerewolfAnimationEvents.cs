using UnityEngine;
using UnityEngine.Events;

public class WerewolfAnimationEvents : MonoBehaviour
{
    public UnityEvent OnCatchTriggerFrame { get; private set; } = new();
    public UnityEvent OnLastCatchFrame { get; private set; } = new();
    public UnityEvent OnLastKitchenIdleFrame { get; private set; } = new();
    public UnityEvent OnLastKitchenNoticeFrame { get; private set; } = new();

    private void Animator_OnCatchTriggerFrame()
    {
        OnCatchTriggerFrame.Invoke();
    }

    private void Animator_OnLastCatchFrame()
    {
        OnLastCatchFrame.Invoke();
    }

    private void Animator_OnLastKitchenIdleFrame()
    {
        OnLastKitchenIdleFrame.Invoke();
    }

    private void Animator_OnLastKitchenNoticeFrame()
    {
        OnLastKitchenNoticeFrame.Invoke();
    }
}
