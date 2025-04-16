using UnityEngine;

/// <summary>
/// Adds quest advancement capability to an game object. When player finishes his interaction with this game object,
/// sub quest will be completed.
/// </summary>
public class QuestAdvancer : MonoBehaviour, IInteractable
{
    private void Awake()
    {
        enabled = false;
    }

    public void Interact(PlayerController player)
    {
        if (!enabled)
            return;

        ChildController.Instance.FinishSubQuest();
    }
}
