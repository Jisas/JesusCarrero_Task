using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Project/Dialogue/DialogueNode")]
public class DialogueSO : ScriptableObject
{
    public string speakerName;
    [TextArea(3, 10)] public string[] lines;
}