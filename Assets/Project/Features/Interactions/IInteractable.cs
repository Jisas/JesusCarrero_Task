
public enum InteractionType { Pickup, Talk, Buy }
public interface IInteractable
{
    InteractionType Type { get; }
    string InteractionPrompt { get; } // Text that will appear in the UI (e.g., “Talk,” “Pick up”)
    void Interact(PlayerController player);
}