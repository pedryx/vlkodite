using UnityEngine;

// Attach to any world object you want to see text on.
public class Interactable : MonoBehaviour
{
    [Tooltip("What text should appear when the player is near?")]
    public string prompt = "Interact";
}
