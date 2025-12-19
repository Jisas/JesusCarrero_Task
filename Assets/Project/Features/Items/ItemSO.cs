using UnityEngine;

public abstract class ItemSO : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    [SerializeField] private string id;
    public Sprite icon;
    public GameObject worldPrefab;
    [TextArea] public string description;
    public bool isStackable;
    public int maxStack = 99;

    public string ID => id;

    [ContextMenu("Generate New ID")]
    private void GenerateID() => id = System.Guid.NewGuid().ToString();

    // Strategy Pattern: Each item defines what it does when used.
    public abstract bool Use();
}