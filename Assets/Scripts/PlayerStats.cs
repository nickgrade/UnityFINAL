using UnityEngine;
using UnityEngine.Events;
using TMPro;                // TextMeshPro namespace
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Health), typeof(PlayerShooting))]
public class PlayerStats : MonoBehaviour
{
    [Header("References (assign in Inspector)")]
    public Health              health;              
    public PlayerShooting      shooter;             
    public TextMeshProUGUI     deathCounterText;    
    public TextMeshProUGUI     keyCounterText;      // ← new field for keys

    [Header("Keys & Events")]
    public int                 keysCollected = 0;
    public UnityEvent          onKeysChanged;

    bool isEasyMode => PlayerPrefs.GetInt("EasyMode", 0) == 1;

    void Awake()
    {
        health = health ?? GetComponent<Health>();
        if (health == null)
        {
            Debug.LogError("Health component is missing on the PlayerStats object!");
        }

        shooter = shooter ?? GetComponent<PlayerShooting>();
        if (shooter == null)
        {
            Debug.LogError("PlayerShooting component is missing on the PlayerStats object!");
        }
    }

    void Start()
    {
        // Death counter setup
        if (deathCounterText != null)
        {
            deathCounterText.gameObject.SetActive(isEasyMode);
        }
        else
        {
            Debug.LogWarning("DeathCounterText is not assigned in PlayerStats!");
        }

        health.onDeath.AddListener(HandlePlayerDeath);
        if (isEasyMode)
        {
            UpdateDeathCounterUI();
        }

        // Key counter setup
        if (keyCounterText != null)
        {
            keyCounterText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("KeyCounterText is not assigned in PlayerStats!");
        }

        onKeysChanged.AddListener(UpdateKeyCounterUI);
        UpdateKeyCounterUI(); // initial display

        // Health-changed UI hookup remains...
        if (health != null)
        {
            health.onHealthChanged.AddListener(() => Debug.Log("Health changed!"));
        }
    }

    void Update()
    {
        // Check for player death
        if (health.IsDead)
        {
            if (PlayerPrefs.GetInt("EasyMode", 0) == 1)
            {
                // Reset health and increment death counter
                health.RestoreFullHealth(); // Use the RestoreFullHealth method
                int deathCount = PlayerPrefs.GetInt("DeathCount", 0) + 1;
                PlayerPrefs.SetInt("DeathCount", deathCount);
                UpdateDeathCounterUI();
            }
            else
            {
                // Normal mode: Handle game over
                Debug.Log("Player has died!");
            }
        }
    }

    void HandlePlayerDeath()
    {
        if (isEasyMode)
        {
            health.TakeDamage(-health.maxHP);
            int deaths = PlayerPrefs.GetInt("DeathCount", 0) + 1;
            PlayerPrefs.SetInt("DeathCount", deaths);
            PlayerPrefs.Save();
            UpdateDeathCounterUI();
        }
        else
        {
            Debug.Log("Player died! Game Over.");
        }
    }

    void UpdateDeathCounterUI()
    {
        if (deathCounterText != null)
            deathCounterText.text = $"Deaths: {PlayerPrefs.GetInt("DeathCount",0)}";
    }

    /// <summary>
    /// Increments your key count and fires the onKeysChanged event.
    /// </summary>
    public void AddKeys(int n)
    {
        keysCollected += n;
        onKeysChanged?.Invoke();
    }

    /// <summary>
    /// Called whenever onKeysChanged fires.
    /// </summary>
    void UpdateKeyCounterUI()
    {
        if (keyCounterText != null)
            keyCounterText.text = $"Keys: {keysCollected}";
    }

    // ... other methods unchanged ...
}
