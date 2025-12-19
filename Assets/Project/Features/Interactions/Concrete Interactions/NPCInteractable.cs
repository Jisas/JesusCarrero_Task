using UnityEngine;

public class NPCInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueSO dialogueData;

    public InteractionType Type => InteractionType.Talk;
    public string InteractionPrompt => $"Hablar con {dialogueData.speakerName}";

    public void Interact(PlayerController player)
    {
        var dialogueManager = ServiceLocator.Get<DialogueManager>();

        if (!dialogueManager.IsDialogueActive)
        {
            dialogueManager.StartDialogue(dialogueData);
        }
    }
}