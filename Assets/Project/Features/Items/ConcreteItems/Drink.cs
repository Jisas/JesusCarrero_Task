using UnityEngine;

[CreateAssetMenu(fileName = "New Drink", menuName = "Project/Items/Drink")]
public class Drink : ItemSO
{
    public override void Use()
    {
        var playerController = ServiceLocator.Get<PlayerController>();
        var playerStats = ServiceLocator.Get<PlayerStats>();

        playerController.TransitionTo(playerController.Drink);
        playerStats.Heal(value);
    }
}
