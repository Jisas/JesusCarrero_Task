using UnityEngine;

public abstract class ItemSO : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    [SerializeField] private string id;
    [TextArea] public string description;
    public Sprite icon;
    public bool isStackable;
    public int maxStack = 99;

    public string ID => id;

    [ContextMenu("Generate New ID")]
    private void GenerateID() => id = System.Guid.NewGuid().ToString();

    // Patrón Strategy: Cada ítem define qué hace al usarse
    public abstract bool Use();
}