using UnityEngine;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapons Setup")]
    public List<GameObject> allWeaponObjects; // All weapons already on the player, disabled by default

    public List<GameObject> ownedWeapons = new List<GameObject>();
    private int currentWeaponIndex = 0;

    void Start()
    {
        if (allWeaponObjects.Count > 0)
        {
            AddWeapon(allWeaponObjects[0]); // First weapon (pistol)
            EquipWeapon(0);
        }
        else
        {
            Debug.LogWarning("WeaponManager: No weapons assigned in allWeaponObjects.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchToSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchToSlot(1);
    }

    public void AddWeapon(GameObject weaponObject)
    {
        if (weaponObject == null)
        {
            Debug.LogWarning("WeaponManager: Tried to add a null weapon.");
            return;
        }

        // Match against actual weapon prefab
        GameObject realWeapon = allWeaponObjects.Find(w => w.name == weaponObject.name);

        if (realWeapon == null)
        {
            Debug.LogWarning("WeaponManager: MysteryBox weapon not found in allWeaponObjects. Using raw reference.");
            realWeapon = weaponObject;
        }

        // If player already owns it, refill its ammo and equip
        if (ownedWeapons.Contains(realWeapon))
        {
            Debug.Log("WeaponManager: Player already owns " + realWeapon.name + ". Refilling ammo.");

            if (realWeapon.TryGetComponent(out MonoBehaviour gunScript))
            {
                var refill = gunScript.GetType().GetMethod("RefillAmmo");
                Debug.Log("Attempting to refill: " + realWeapon.name + " | Active: " + realWeapon.activeInHierarchy);

                refill?.Invoke(gunScript, null);
            }

            EquipWeapon(ownedWeapons.IndexOf(realWeapon));
            return;
        }

        // Otherwise, add or replace
        if (ownedWeapons.Count < 2)
        {
            ownedWeapons.Add(realWeapon);
            Debug.Log("WeaponManager: Added new weapon: " + realWeapon.name);
        }
        else
        {
            Debug.Log("WeaponManager: Replacing weapon in slot " + currentWeaponIndex + " with " + realWeapon.name);
            ownedWeapons[currentWeaponIndex].SetActive(false);
            ownedWeapons[currentWeaponIndex] = realWeapon;
        }

        EquipWeapon(ownedWeapons.IndexOf(realWeapon));
        RefillUnusedWeapons(); // Refill everything else you're not holding
    }


    public void EquipWeapon(int slot)
    {
        if (slot < 0 || slot >= ownedWeapons.Count)
        {
            Debug.LogWarning("WeaponManager: Tried to equip invalid slot: " + slot);
            return;
        }

        foreach (GameObject weapon in allWeaponObjects)
        {
            if (weapon != null)
                weapon.SetActive(false);
        }

        ownedWeapons[slot].SetActive(true);
        currentWeaponIndex = slot;
    }

    public void RefillUnusedWeapons()
    {
        foreach (GameObject weapon in allWeaponObjects)
        {
            if (weapon == null) continue;

            // Skip weapons the player currently owns
            if (ownedWeapons.Contains(weapon)) continue;

            if (weapon.TryGetComponent(out MonoBehaviour gunScript))
            {
                var refill = gunScript.GetType().GetMethod("RefillAmmo");
                refill?.Invoke(gunScript, null);
            }
        }
    }


    private void SwitchToSlot(int slot)
    {
        if (slot < ownedWeapons.Count)
        {
            EquipWeapon(slot);
        }
    }

    public GameObject GetCurrentWeapon()
    {
        return ownedWeapons.Count > 0 ? ownedWeapons[currentWeaponIndex] : null;
    }

    public int GetWeaponCount()
    {
        return ownedWeapons.Count;
    }

    public bool HasWeapon(GameObject weaponObject)
    {
        return ownedWeapons.Contains(weaponObject);
    }
}
