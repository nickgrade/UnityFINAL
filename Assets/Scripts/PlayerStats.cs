using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Health), typeof(PlayerShooting))]
public class PlayerStats : MonoBehaviour
{
    [Header("References")]
    public Health health;
    public PlayerShooting shooter;

    [Header("Keys")]
    public int keysCollected = 0;
    public UnityEvent onKeysChanged;

    void Awake()
    {
        health  = health  ?? GetComponent<Health>();
        shooter = shooter ?? GetComponent<PlayerShooting>();
    }

    public void HealPercent(float pct)
    {
        int heal = Mathf.CeilToInt(health.maxHP * pct);
        health.TakeDamage(-heal);
    }

    public void AddMaxHealth(float extraBars)
    {
        int extra = Mathf.CeilToInt(extraBars);
        health.maxHP += extra;
        health.TakeDamage(-extra); // Heal by the added amount to reflect new maxHP
    }

    public void ModifyFireRate(float delta)
    {
        shooter.fireRate = Mathf.Max(0.05f, shooter.fireRate + delta);
    }

    public void ModifyDamage(float delta)
    {
        shooter.bulletDamage += Mathf.CeilToInt(delta);
    }

    public void AddKeys(float n)
    {
        keysCollected += Mathf.CeilToInt(n);
        onKeysChanged?.Invoke();
    }
}
