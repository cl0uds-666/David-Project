using UnityEngine;

public class PowerupSpawner : MonoBehaviour
{
    [System.Serializable]
    public class PowerupChance
    {
        public GameObject prefab;
        public float chance; // Between 0 and 1
    }

    public PowerupChance[] powerups;

    public void TrySpawnPowerup(Vector3 position)
    {
        foreach (var p in powerups)
        {
            if (Random.value <= p.chance)
            {
                Instantiate(p.prefab, position + Vector3.up * 0.5f, Quaternion.identity);
                break; // Only spawn one
            }
        }
    }
}
