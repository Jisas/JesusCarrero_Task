using UnityEngine;

public class EntityInputReader : MonoBehaviour
{
    [Header("Input Asset Reference")]
    [SerializeField] private InputReaderSO inputReader;

    [Header("Settings")]
    [SerializeField] private bool lockCursorOnStart = true;

    // Property for easy access by PlayerController
    public InputReaderSO Input => inputReader;

    private void Start()
    {
        // We set the initial state
        if (lockCursorOnStart)
        {
            SetCursorState(true);
        }
        inputReader.SwitchToPlayerInput();
    }

    public void SetCursorState(bool isLocked)
    {
        // In Isometric: Blocked = Invisible
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isLocked;
    }

    // Convenient methods for calling from the FSM
    public void SwitchToUI()
    {
        SetCursorState(false);
        inputReader.SwitchToUIInput();
    }

    public void SwitchToPlayer()
    {
        SetCursorState(true);
        inputReader.SwitchToPlayerInput();
    }
}