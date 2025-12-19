using UnityEngine;

public class PlayerStats : EntityStats, IGameService
{
    public override void Initialize()
    {
        currentHealth = maxHealth;
    }

    public override void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"Current health: {currentHealth}");
        OnHeal?.Invoke();
    }

    public override void TakeDamage(float amount)
    {
        currentHealth = Mathf.Min(currentHealth - amount, maxHealth);
        Debug.Log($"Current health: {currentHealth}");
        OnTakeDamage?.Invoke();
    }
}
