using UnityEngine;
using UnityEngine.Rendering.Universal; // for Light2D

public class DelayedLight2DActivation : MonoBehaviour
{
    [SerializeField] private Light2D light2D;         // Assign the Light2D component in Inspector
    [SerializeField] private float delayBeforeEnable = 1f;

    private void Awake()
    {
        if (light2D == null)
            light2D = GetComponent<Light2D>();

        if (light2D != null)
            light2D.enabled = false; // Start disabled
    }

    private void OnEnable()
    {
        StartCoroutine(EnableLightAfterDelay());
    }

    private System.Collections.IEnumerator EnableLightAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeEnable);
        if (light2D != null)
            light2D.enabled = true;
    }
}
