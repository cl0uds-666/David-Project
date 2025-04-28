using UnityEngine;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapons Setup")]
    public List<GameObject> allWeaponObjects; // All weapons already on the player, disabled by default

    private List<GameObject> ownedWeapons = new List<GameObject>();
    private int currentWeaponIndex = 0;

    void Start()
    {
        if (allWeaponObjects.Count > 0)
        {
            AddWeapon(allWeaponObjects[0]);
            EquipWeapon(0);
        }
        else
        {
            Debug.LogWarning("No weapons assigned to WeaponManager.");
        }

        // TEMP FOR TESTING: Add Sledgehammer
        foreach (GameObject weapon in allWeaponObjects)
        {
            if (weapon != null && weapon.name.ToLower().Contains("sledge"))
            {
                AddWeapon(weapon);
                Debug.Log("Added Sledgehammer for testing.");
                break;
            }
        }
    }


    void Update()
    {
        // Switch weapons with number keys (1 and 2)
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchToSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchToSlot(1);
    }

    public void AddWeapon(GameObject weaponObject)
    {
        if (weaponObject == null)
        {
            Debug.LogWarning("Tried to add a null weapon.");
            return;
        }

        if (ownedWeapons.Contains(weaponObject))
        {
            Debug.Log("Player already owns: " + weaponObject.name);
            return;
        }

        if (ownedWeapons.Count < 2)
        {
            ownedWeapons.Add(weaponObject);
        }
        else
        {
            // Replace the current weapon if inventory is full
            ownedWeapons[currentWeaponIndex].SetActive(false);
            ownedWeapons[currentWeaponIndex] = weaponObject;
        }

        EquipWeapon(ownedWeapons.IndexOf(weaponObject));
    }

    private void EquipWeapon(int slot)
    {
        if (slot < 0 || slot >= ownedWeapons.Count)
        {
            Debug.LogWarning("Tried to equip invalid weapon slot: " + slot);
            return;
        }

        // Disable all weapons first
        foreach (GameObject weapon in allWeaponObjects)
        {
            if (weapon != null)
                weapon.SetActive(false);
        }

        // Enable selected weapon
        ownedWeapons[slot].SetActive(true);
        currentWeaponIndex = slot;
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
