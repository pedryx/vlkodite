using UnityEngine;
using UnityEngine.Events;

public class WerewolfAnimationEvents : MonoBehaviour
{
    #region Catch mechanic
    [Header("Catch mechanic")]
    [field: SerializeField]
    public UnityEvent OnCatchTriggerFrame { get; private set; } = new();

    [field: SerializeField]
    public UnityEvent OnLastCatchFrame { get; private set; } = new();

    private void Animator_OnCatchTriggerFrame()
    {
        OnCatchTriggerFrame.Invoke();
    }

    private void Animator_OnLastCatchFrame()
    {
        OnLastCatchFrame.Invoke();
    }
    #endregion
    #region Kitchen encounter
    [Header("Kitchen encounter")]
    [field: SerializeField]
    public UnityEvent OnLastKitchenIdleFrame { get; private set; } = new();

    [field: SerializeField]
    public UnityEvent OnLastKitchenNoticeFrame { get; private set; } = new();

    private void Animator_OnLastKitchenIdleFrame()
    {
        OnLastKitchenIdleFrame.Invoke();
    }

    private void Animator_OnLastKitchenNoticeFrame()
    {
        OnLastKitchenNoticeFrame.Invoke();
    }
    #endregion
    #region Night 2 transform
    [Header("Night 2 transform")]
    [field: SerializeField]
    public UnityEvent OnLastTransformFrame { get; private set; } = new();

    private void Animator_OnLastTransformFrame()
    {
        OnLastTransformFrame.Invoke();
    }
    #endregion
}
