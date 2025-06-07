using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public void OnExitPressed()
    {
        Application.Quit();
        Debug.Log("Quit Game"); // For editor testing
    }
}
