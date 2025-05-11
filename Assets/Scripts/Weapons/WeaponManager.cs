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

        // Try to match against known weapon references
        GameObject realWeapon = allWeaponObjects.Find(w => w.name == weaponObject.name);

        if (realWeapon == null)
        {
            Debug.LogWarning("WeaponManager: MysteryBox weapon not found in allWeaponObjects. Trying to add it anyway...");
            realWeapon = weaponObject;
        }
        else
        {
            Debug.Log("WeaponManager: Using matched prefab from allWeaponObjects: " + realWeapon.name);
        }

        if (ownedWeapons.Contains(realWeapon))
        {
            Debug.Log("WeaponManager: Player already owns " + realWeapon.name);
            EquipWeapon(ownedWeapons.IndexOf(realWeapon));
            return;
        }

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


        if (ownedWeapons.Count < 2)
        {
            ownedWeapons.Add(weaponObject);
            Debug.Log("WeaponManager: Added new weapon: " + weaponObject.name);
        }
        else
        {
            Debug.Log("WeaponManager: Replacing weapon in slot " + currentWeaponIndex + " with " + weaponObject.name);
            ownedWeapons[currentWeaponIndex].SetActive(false);
            ownedWeapons[currentWeaponIndex] = weaponObject;
        }

        EquipWeapon(ownedWeapons.IndexOf(weaponObject));
        Debug.Log("WeaponManager: Equipped weapon: " + weaponObject.name);
    }

    private void EquipWeapon(int slot)
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
