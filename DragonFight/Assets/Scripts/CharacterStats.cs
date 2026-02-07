using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages health and survival status.
/// Works for Player, AI, or destructible objects.
/// </summary>
public class CharacterStats : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] public int maxHealth = 100;

    // Private setter ensures only this script changes health
    public int CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }

    // Events for UI and Animation systems to listen to
    // float = health percentage (0 to 1)
    public event UnityAction<float> OnHealthChanged;
    public event UnityAction OnDeath;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        IsDead = false;
    }

    /// <summary>
    /// Applies damage to the character.
    /// </summary>
    /// <param name="amount">Amount of damage to deal.</param>
    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        CurrentHealth -= amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);

        // Calculate percentage for UI bars (0.0f to 1.0f)
        float healthPercent = (float)CurrentHealth / maxHealth;

        // Invoke event: "Hey listeners, health changed to this %!"
        OnHealthChanged?.Invoke(healthPercent);

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Handles death logic.
    /// </summary>
    private void Die()
    {
        IsDead = true;
        OnDeath?.Invoke();
        Debug.Log($"{gameObject.name} has died.");
        // Logic for game over manager will go here later via event
    }
}