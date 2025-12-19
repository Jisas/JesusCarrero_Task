using UnityEngine;

public class NPCInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string npcName;
    [SerializeField] private string[] dialogueLines;

    public string InteractionPrompt => $"Talk to {npcName}";

    public void Interact(PlayerController player)
    {
        // Trigger dialog system
    }
}