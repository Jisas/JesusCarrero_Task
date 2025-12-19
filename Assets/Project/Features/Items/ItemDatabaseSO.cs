using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Project/Items/Database")]
public class ItemDatabaseSO : ScriptableObject, IGameService
{
    public List<ItemSO> allItems;

    public ItemSO GetItemByID(string id) => allItems.Find(i => i.ID == id);
}