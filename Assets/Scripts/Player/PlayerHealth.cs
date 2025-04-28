using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Damage Overlay")]
    public Image damageOverlay;
    public float fadeSpeed = 2f; // Speed of fade in/out
    private bool isLowHealth = false;

    [Header("Health Settings")]
    public float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    public float regenDelay = 3f;
    public float regenRate = 10f;

    private bool isRegenerating = false;
    public bool isInvincible = false; // Invincibility toggle

    public Transform respawnPoint;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public void SetHealth(float health)
    {
        currentHealth = health;
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        Debug.Log("Player took " + damage + " damage. Current HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StopCoroutine("RegenerateHealth");
            StartCoroutine("RegenerateHealth");
        }

        // Check for low health
        if (currentHealth <= 50f)
        {
            isLowHealth = true;
        }
        else
        {
            isLowHealth = false;
        }
    }

    void Update()
    {
        HandleDamageOverlay();
    }

    void HandleDamageOverlay()
    {
        if (damageOverlay == null) return;

        // Always check live if low health
        isLowHealth = currentHealth <= 50f;

        Color color = damageOverlay.color;
        float targetAlpha = isLowHealth ? 0.4f : 0f;

        color.a = Mathf.Lerp(color.a, targetAlpha, fadeSpeed * Time.deltaTime);
        damageOverlay.color = color;
    }



    IEnumerator RegenerateHealth()
    {
        isRegenerating = true;
        Debug.Log("Regeneration starting in " + regenDelay + " seconds...");

        yield return new WaitForSeconds(regenDelay);

        while (currentHealth < maxHealth)
        {
            currentHealth += regenRate * Time.deltaTime;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            yield return null;
        }

        Debug.Log("Health fully regenerated!");
        isRegenerating = false;
    }

    void Die()
    {
        Debug.Log("Player Died! Respawning...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        currentHealth = maxHealth;
    }
}
