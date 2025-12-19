using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class DialogueDisplay : MonoBehaviour, IInitializable
{
    [Header("References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI lineText;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.03f;

    private Coroutine _typingCoroutine;
    private DialogueManager _manager;

    public void Initialize()
    {
        // 1. Get the ServiceLocator manager
        _manager = ServiceLocator.Get<DialogueManager>();

        // 2. Subscription to events
        _manager.OnDialogueStarted += ShowDialogue;
        _manager.OnLineChanged += UpdateLine;
        _manager.OnDialogueEnded += HideDialogue;

        // Hide the panel on startup
        dialoguePanel.SetActive(false);
    }

    private void ShowDialogue(string speakerName, string firstLine)
    {
        dialoguePanel.SetActive(true);
        nameText.text = speakerName;

        UpdateLine(firstLine);
    }

    private void UpdateLine(string line)
    {
        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
        _typingCoroutine = StartCoroutine(TypeText(line));
    }

    private IEnumerator TypeText(string line)
    {
        lineText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            lineText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        _typingCoroutine = null;
    }

    private void HideDialogue()
    {
        if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
        dialoguePanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (_manager != null)
        {
            _manager.OnDialogueStarted -= ShowDialogue;
            _manager.OnLineChanged -= UpdateLine;
            _manager.OnDialogueEnded -= HideDialogue;
        }
    }
}
