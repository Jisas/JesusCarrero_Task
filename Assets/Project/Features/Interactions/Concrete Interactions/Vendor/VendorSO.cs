using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VendorSO", menuName = "Project/Vendor")]
public class VendorSO : ScriptableObject
{
    [SerializeField] private List<ItemSO> availableItems = new();

    public List<ItemSO> AvaiableItems => availableItems;
}
