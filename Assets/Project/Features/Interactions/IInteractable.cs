
public interface IInteractable
{
    string InteractionPrompt { get; } // Text that will appear in the UI (e.g., “Talk,” “Pick up”)
    void Interact(PlayerController player);
}