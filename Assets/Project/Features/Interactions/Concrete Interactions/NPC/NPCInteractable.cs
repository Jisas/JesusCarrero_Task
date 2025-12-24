using UnityEngine;

public class NPCInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueSO dialogueData;

    public InteractionType Type => InteractionType.Talk;
    public string InteractionPrompt => $"Talk To {dialogueData.speakerName}";

    public void Interact(PlayerController player)
    {
        var dialogueManager = ServiceLocator.Get<DialogueManager>();

        if (!dialogueManager.IsDialogueActive)
        {
            dialogueManager.StartDialogue(dialogueData);
        }
    }
}