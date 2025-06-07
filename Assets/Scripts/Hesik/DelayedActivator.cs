using UnityEngine;

public class DelayedActivator : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private float delay = 1f;

    private void OnEnable()
    {
        if (target != null)
        {
            target.SetActive(false); // Ensure it's off at first
            Invoke(nameof(ActivateTarget), delay);
        }
    }

    private void ActivateTarget()
    {
        target.SetActive(true);
    }
}
