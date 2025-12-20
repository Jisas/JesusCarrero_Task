using UnityEngine;

public class PlayerStats : EntityStats, IInitializable, IGameService
{
    public void Initialize()
    {
        currentHealth = maxHealth / 2;
        Debug.Log($"<color=green>Max health: {maxHealth}</color>");
        Debug.Log($"<color=green>Current health: {currentHealth}</color>");
    }

    public override void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"<color=green>Current health: {currentHealth}</color>");
        OnHeal?.Invoke();
    }

    public override void TakeDamage(float amount)
    {
        currentHealth = Mathf.Min(currentHealth - amount, maxHealth);
        Debug.Log($"<color=red>Taken damage: {amount}</color>");
        Debug.Log($"<color=red>Current health: {currentHealth}</color>");
        OnTakeDamage?.Invoke();
    }
}
