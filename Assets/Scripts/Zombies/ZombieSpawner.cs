using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using System.Linq;


public class ZombieSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject zombiePrefab;
    public Transform[] spawnPoints;
    public int baseZombiesPerRound = 6;
    public float spawnDelay = 2f;

    [Header("Round UI")]
    public TextMeshProUGUI roundText;
    [SerializeField] private UIManager uiManager;


    private int currentRound = 0;
    private int killsNeeded = 0;   // player kills required this round
    private int killsSoFar = 0;   // player kills achieved
    private int aliveZombies = 0;   // total zombies currently alive

    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();

        StartNextRound();
    }

    // -------------------------------------------------
    void StartNextRound()
    {
        currentRound++;

        if (uiManager != null)
            uiManager.SetRound(currentRound);
        else
            roundText.text = "Round " + currentRound;


        killsNeeded = baseZombiesPerRound + (currentRound * 2);
        killsSoFar = 0;
        aliveZombies = 0;

        StartCoroutine(SpawnZombies());
    }

    IEnumerator SpawnZombies()
    {
        for (int i = 0; i < killsNeeded; i++)
        {
            SpawnZombie();
            yield return new WaitForSeconds(spawnDelay);
        }
    }


    void SpawnZombie()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null || spawnPoints.Length < 3)
        {
            Debug.LogWarning("ZombieSpawner: No player found or not enough spawn points. Spawning at random.");
            SpawnAtRandom();
            return;
        }

        var nearestSpawns = GetClosestSpawnPoints(player.position, 3);
        Transform sp = nearestSpawns[Random.Range(0, nearestSpawns.Count)];

        GameObject z = Instantiate(zombiePrefab, sp.position, sp.rotation);

        EnemyHealth eh = z.GetComponent<EnemyHealth>();
        eh.onDeath += ZombieDied;

        aliveZombies++;
    }


    void SpawnAtRandom()
    {
        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject z = Instantiate(zombiePrefab, sp.position, sp.rotation);

        EnemyHealth eh = z.GetComponent<EnemyHealth>();
        eh.onDeath += ZombieDied;

        aliveZombies++;
    }

    // -------------------------------------------------
    void ZombieDied(EnemyHealth z, bool countAsKill)
    {
        aliveZombies--;

        if (countAsKill)
        {
            killsSoFar++;
        }
        else
        {
            SpawnZombie();
        }

        // start next round only when required kills met AND no zombies alive
        if (killsSoFar >= killsNeeded && aliveZombies <= 0)
            StartCoroutine(WaitForNextRound());
    }

    IEnumerator WaitForNextRound()
    {
        if (uiManager != null)
            yield return uiManager.PulseUntilRoundStart(5f, currentRound + 1);
        else
            yield return new WaitForSeconds(5f);

        StartNextRound();

    }

    public int GetCurrentRound()
    {
        return currentRound;
    }

    public List<Transform> GetClosestSpawnPoints(Vector3 playerPosition, int count)
    {
        return spawnPoints
            .OrderBy(sp => Vector3.Distance(sp.position, playerPosition))
            .Take(count)
            .ToList();
    }
}
