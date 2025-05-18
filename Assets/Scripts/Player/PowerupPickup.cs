using UnityEngine;

public class PowerupPickup : MonoBehaviour
{
    public enum PowerupType { MaxAmmo, InstaKill, Nuke, DoublePoints, Pause }
    public PowerupType type;

    public float duration = 10f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (type)
            {
                case PowerupType.MaxAmmo:
                    WeaponManager wm = FindObjectOfType<WeaponManager>();
                    wm?.RefillUnusedWeapons(); // And refill current weapons too
                    var currentWeapon = wm?.GetCurrentWeapon();
                    if (currentWeapon.TryGetComponent(out MonoBehaviour gunScript))
                    {
                        var refill = gunScript.GetType().GetMethod("RefillAmmo");
                        refill?.Invoke(gunScript, null);
                    }
                    break;

                case PowerupType.InstaKill:
                    PowerupEffects.Instance?.ActivateInstaKill(duration);
                    Debug.Log("Insta-Kill triggered!");
                    break;

                case PowerupType.Nuke:
                    PowerupEffects.Instance?.TriggerNuke();
                    break;

                case PowerupType.DoublePoints:
                    PowerupEffects.Instance?.ActivateDoublePoints(duration);
                    break;

                case PowerupType.Pause:
                    PowerupEffects.Instance?.ActivatePause(duration);
                    break;


            }

            Destroy(gameObject);
        }
    }
}
