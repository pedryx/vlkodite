using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class ZoomTrigger : MonoBehaviour
{
    public CinemachineCamera zoomCamera;
    public CinemachineCamera playerCamera;

    private static Stack<ZoomTrigger> triggerStack = new Stack<ZoomTrigger>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        triggerStack.Push(this);
        UpdateCameraPriority();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (triggerStack.Contains(this))
        {
            // Create a new stack without this trigger
            Stack<ZoomTrigger> newStack = new Stack<ZoomTrigger>();
            while (triggerStack.Count > 0)
            {
                var popped = triggerStack.Pop();
                if (popped != this)
                    newStack.Push(popped);
            }

            // Rebuild the stack in correct order
            while (newStack.Count > 0)
            {
                triggerStack.Push(newStack.Pop());
            }

            UpdateCameraPriority();
        }
    }

    private static void UpdateCameraPriority()
    {
        
       var allTriggers = Object.FindObjectsByType<ZoomTrigger>(FindObjectsSortMode.None);

        foreach (var trig in allTriggers)
        {
            trig.zoomCamera.Priority = 10;
            trig.playerCamera.Priority = 10;
        }

        if (triggerStack.Count > 0)
        {
            var activeTrigger = triggerStack.Peek();
            activeTrigger.zoomCamera.Priority = 11;
            activeTrigger.playerCamera.Priority = 10;
        }
    }


}
