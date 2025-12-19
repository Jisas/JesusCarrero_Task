using UnityEngine;
using System;

public class DialogueManager : MonoBehaviour, IGameService
{
    public event Action<string, string> OnDialogueStarted;
    public event Action<string> OnLineChanged;
    public event Action OnDialogueEnded;

    private DialogueSO _currentDialogue;
    private int _currentLineIndex;

    public bool IsDialogueActive { get; private set; }

    public void StartDialogue(DialogueSO dialogue)
    {
        _currentDialogue = dialogue;
        _currentLineIndex = 0;
        IsDialogueActive = true;

        OnDialogueStarted?.Invoke(dialogue.speakerName, dialogue.lines[0]);

        Debug.Log("Inicio el dialogo");
    }

    public void AdvanceDialogue()
    {
        _currentLineIndex++;

        if (_currentLineIndex < _currentDialogue.lines.Length)
        {
            OnLineChanged?.Invoke(_currentDialogue.lines[_currentLineIndex]);
            Debug.Log("Avanzo el dialogo");
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        IsDialogueActive = false;
        OnDialogueEnded?.Invoke();
        Debug.Log("Termino el dialogo");
    }
}