using UnityEngine;

[CreateAssetMenu(fileName = "TestItem", menuName = "Project/Items/TestItem")]
public class HealthPotion : ItemSO
{
    public override void Use()
    {
        var playerStats = ServiceLocator.Get<PlayerStats>();
        playerStats.Heal(value);
    }
}
