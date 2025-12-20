using UnityEngine;
using System;

public class EntityStats : MonoBehaviour
{
    public float maxHealth = 100;
    public float currentHealth;

    public Action OnHeal;
    public Action OnTakeDamage;

    public virtual void Heal(float amount) { }
    public virtual void TakeDamage(float amount) { }
}
