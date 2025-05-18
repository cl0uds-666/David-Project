using UnityEngine;

public class PowerupEffects : MonoBehaviour
{
    public static PowerupEffects Instance;

    private bool instaKillActive = false;
    private float instaKillEndTime = 0f;

    private bool doublePointsActive = false;
    private float doublePointsEndTime = 0f;

    private bool pauseActive = false;
    private float pauseEndTime = 0f;



    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (instaKillActive && Time.time >= instaKillEndTime)
        {
            instaKillActive = false;
            Debug.Log("Insta-Kill ended");
        }
        if (doublePointsActive && Time.time >= doublePointsEndTime)
        {
            doublePointsActive = false;
            Debug.Log("Double Points Ended.");
        }

        if (pauseActive && Time.time >= pauseEndTime)
        {
            pauseActive = false;
            Debug.Log("Pause Ended!");

            ZombieAI[] zombies = FindObjectsOfType<ZombieAI>();
            foreach (ZombieAI zombie in zombies)
            {
                zombie.SetPaused(false);
            }
        }




    }

    public void ActivateInstaKill(float duration)
    {
        instaKillActive = true;
        instaKillEndTime = Time.time + duration;
        Debug.Log("Insta-Kill Activated!");
    }

    public bool IsInstaKill()
    {
        Debug.Log("IsInstaKill() checked — returning " + instaKillActive);
        return instaKillActive;
    }

    public void TriggerNuke()
    {
        Debug.Log("NUKE Triggered!");

        EnemyHealth[] zombies = FindObjectsOfType<EnemyHealth>();
        foreach (EnemyHealth zombie in zombies)
        {
            if (zombie != null && zombie.gameObject.activeInHierarchy)
            {
                zombie.TakeDamage(zombie.currentHealth + 1f, null);
            }
        }

        
    }

    public void ActivateDoublePoints(float duration)
    {
        doublePointsActive = true;
        doublePointsEndTime = Time.time + duration;
        Debug.Log("Double Points Activated!");
    }

    public bool IsDoublePoints()
    {
        return doublePointsActive;
    }

    public void ActivatePause(float duration)
    {
        pauseActive = true;
        pauseEndTime = Time.time + duration;
        Debug.Log("Pause Activated!");

        ZombieAI[] zombies = FindObjectsOfType<ZombieAI>();
        foreach (ZombieAI zombie in zombies)
        {
            zombie.SetPaused(true);
        }
    }

    public bool IsPaused()
    {
        return pauseActive;
    }


}
