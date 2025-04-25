using UnityEngine;


/// <summary>
/// Adds quest advancement capability to an game object. When player finishes his interaction with this game object,
/// sub quest will be completed.
/// </summary>
[RequireComponent(typeof(Interactable))]
public class QuestAdvancer : MonoBehaviour
{
    private Interactable interactable;

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
        interactable.InteractionEnabled = false;
        interactable.OnInteract.AddListener(Interactable_OnInteract);

        var child = ChildController.Instance;
        child.OnSubQuestStart.AddListener(Child_OnSubQuestStart);
        child.OnSubQuestDone.AddListener(Child_OnSubQuestDone);
    }

    private void Interactable_OnInteract(Interactable interactable) => ChildController.Instance.FinishSubQuest();

    private void Child_OnSubQuestStart(SubQuestEventArgs e)
    {
        if (e.SubQuest.QuestAdvancer == this)
            interactable.InteractionEnabled = true;
    }

    private void Child_OnSubQuestDone(SubQuestEventArgs e)
    {
        if (e.SubQuest.QuestAdvancer == this)
            interactable.InteractionEnabled = false;
    }
}