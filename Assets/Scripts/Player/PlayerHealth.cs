using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public float regenDelay = 3f; // Time before regen starts
    public float regenRate = 10f; // HP per second when regenerating

    private bool isRegenerating = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
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
            currentHealth = Mathf.Min(currentHealth, maxHealth); // Prevent overhealing
            yield return null;
        }

        Debug.Log("Health fully regenerated!");
        isRegenerating = false;
    }

    public Transform respawnPoint;

    void Die()
    {
        Debug.Log("Player Died! Respawning...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        currentHealth = maxHealth; // Reset health
    }

}
