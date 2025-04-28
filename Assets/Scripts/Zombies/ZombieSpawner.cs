using UnityEngine;
using System.Collections;
using TMPro; // Import TMP

public class ZombieSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject zombiePrefab;
    public Transform[] spawnPoints;
    public int baseZombiesPerRound = 6; // Initial zombies in Round 1
    public float spawnDelay = 2f;

    [Header("Round Settings")]
    public TextMeshProUGUI roundText; // Use TMP instead of UI Text
    private int currentRound = 0;
    private int zombiesRemaining;
    private int zombiesToSpawn;

    void Start()
    {
        StartNextRound();
    }

    void StartNextRound()
    {
        currentRound++;
        roundText.text = "Round " + currentRound; // Update TMP UI
        zombiesToSpawn = baseZombiesPerRound + (currentRound * 2); // Increase zombies per round
        zombiesRemaining = zombiesToSpawn;

        StartCoroutine(SpawnZombies());
    }

    IEnumerator SpawnZombies()
    {
        for (int i = 0; i < zombiesToSpawn; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject newZombie = Instantiate(zombiePrefab, spawnPoint.position, spawnPoint.rotation);

            // Track zombie deaths
            newZombie.GetComponent<EnemyHealth>().onDeath += ZombieDied;

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    void ZombieDied()
    {
        zombiesRemaining--;

        // When all zombies are dead, start the next round
        if (zombiesRemaining <= 0)
        {
            StartCoroutine(WaitForNextRound());
        }
    }

    IEnumerator WaitForNextRound()
    {
        yield return new WaitForSeconds(3f); // Small delay before next round
        StartNextRound();
    }

    public int GetCurrentRound()
    {
        return currentRound;
    }

}
