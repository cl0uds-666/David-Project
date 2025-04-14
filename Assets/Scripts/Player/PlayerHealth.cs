using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
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
        if (isInvincible) return; // Can't take damage while rewinding

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
