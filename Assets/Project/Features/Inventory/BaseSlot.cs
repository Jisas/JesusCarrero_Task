using UnityEngine;
using System;

[Serializable]
public class BaseSlot : MonoBehaviour
{
    [SerializeField] protected InventorySlotInfo SlotInfo;
    public BaseSlot GetBaseSlot => this;
}
