using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MysteryBoxOpen : MonoBehaviour
{
    [Header("Box References")]
    public GameObject closedBox;
    public GameObject openBox;

    [Header("Weapon Settings")]
    public List<GameObject> weaponObjects;
    public float timeBetweenRises = 0.5f;
    public float riseHeight = 0.5f;
    public float riseSpeed = 2f;
    public float hoverDuration = 2f;

    [Header("Controls")]
    public KeyCode acceptKey = KeyCode.F;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip boxOpenSound;

    private GameObject currentWeapon;
    private bool canAccept = false;
    private int selectedIndex = -1;
    private bool playerInRange = false;

    void Start()
    {
        Debug.Log("MysteryBoxOpen: Started. Beginning roll...");

        if (closedBox == null || openBox == null)
        {
            Debug.LogError("MysteryBoxOpen: Missing open/closed box references!");
        }

        foreach (var w in weaponObjects)
        {
            if (w != null)
                w.SetActive(false);
        }

        StartCoroutine(RollWeapons());
    }

    public void BeginRoll()
    {
        Debug.Log("MysteryBoxOpen: BeginRoll() called manually.");
        StopAllCoroutines(); // Safety
        StartCoroutine(RollWeapons());
    }

    IEnumerator RollWeapons()
    {
        openBox.SetActive(true);
        closedBox.SetActive(false);

        //Play mystery box sound
        if (audioSource != null && boxOpenSound != null)
        {
            audioSource.clip = boxOpenSound;
            audioSource.Play();
        }

        Debug.Log("MysteryBoxOpen: Box opened, cycling weapons...");

        for (int i = 0; i < weaponObjects.Count; i++)
        {
            yield return StartCoroutine(RiseAndFall(weaponObjects[i]));
        }

        // Final weapon
        selectedIndex = Random.Range(0, weaponObjects.Count);
        currentWeapon = weaponObjects[selectedIndex];

        Debug.Log($"MysteryBoxOpen: Selected weapon is {currentWeapon.name}");

        yield return StartCoroutine(RiseAndHover(currentWeapon));

        canAccept = true;
        Debug.Log("MysteryBoxOpen: Player can now accept weapon (press F)");

        yield return new WaitForSeconds(hoverDuration);

        if (canAccept)
        {
            Debug.Log("MysteryBoxOpen: Player didn't accept weapon in time, lowering...");
            StartCoroutine(LowerAndReset());
        }
    }

    void Update()
    {
        if (canAccept && playerInRange && Input.GetKeyDown(acceptKey))
        {
            Debug.Log("MysteryBoxOpen: Player accepted weapon.");

            WeaponManager wm = FindObjectOfType<WeaponManager>();
            if (wm != null && currentWeapon != null)
            {
                GameObject actualWeapon = wm.allWeaponObjects.Find(w => w.name == currentWeapon.name);

                if (actualWeapon != null)
                {
                    Debug.Log("MysteryBoxOpen: Found matching prefab in allWeaponObjects: " + actualWeapon.name);

                    if (wm.ownedWeapons.Contains(actualWeapon))
                    {
                        Debug.Log("MysteryBoxOpen: Player already owns this weapon. Refilling ammo.");

                        if (actualWeapon.TryGetComponent(out MonoBehaviour gunScript))
                        {
                            var refill = gunScript.GetType().GetMethod("RefillAmmo");
                            refill?.Invoke(gunScript, null);
                        }

                        wm.EquipWeapon(wm.ownedWeapons.IndexOf(actualWeapon));
                    }
                    else
                    {
                        wm.AddWeapon(actualWeapon);
                        wm.RefillUnusedWeapons(); // Refill all other guns
                    }
                }
                else
                {
                    Debug.LogWarning("MysteryBoxOpen: No exact match found in allWeaponObjects! Using raw reference: " + currentWeapon.name);
                    wm.AddWeapon(currentWeapon); // fallback
                    wm.RefillUnusedWeapons();
                }

                Debug.Log("MysteryBoxOpen: Weapon logic completed.");
            }


            canAccept = false;
            StartCoroutine(LowerAndReset());
        }
    }

    IEnumerator RiseAndFall(GameObject weapon)
    {
        if (weapon == null)
        {
            Debug.LogWarning("MysteryBoxOpen: Null weapon encountered during roll.");
            yield break;
        }

        weapon.SetActive(true);
        Vector3 start = weapon.transform.localPosition;
        Vector3 end = start + Vector3.up * riseHeight;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * riseSpeed;
            weapon.transform.localPosition = Vector3.Lerp(start, end, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * riseSpeed;
            weapon.transform.localPosition = Vector3.Lerp(end, start, t);
            yield return null;
        }

        weapon.SetActive(false);
    }

    IEnumerator RiseAndHover(GameObject weapon)
    {
        if (weapon == null)
        {
            Debug.LogWarning("MysteryBoxOpen: Null weapon in RiseAndHover.");
            yield break;
        }

        weapon.SetActive(true);
        Vector3 start = weapon.transform.localPosition;
        Vector3 end = start + Vector3.up * riseHeight;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * riseSpeed;
            weapon.transform.localPosition = Vector3.Lerp(start, end, t);
            yield return null;
        }

        weapon.transform.localPosition = end;
    }

    IEnumerator LowerAndReset()
    {
        if (currentWeapon == null)
        {
            Debug.LogWarning("MysteryBoxOpen: No weapon to lower.");
            yield break;
        }

        Vector3 start = currentWeapon.transform.localPosition;
        Vector3 end = start - Vector3.up * riseHeight;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * riseSpeed;
            currentWeapon.transform.localPosition = Vector3.Lerp(start, end, t);
            yield return null;
        }

        currentWeapon.SetActive(false);
        canAccept = false;

        openBox.SetActive(false);
        MysteryBoxClosed boxScript = closedBox.GetComponent<MysteryBoxClosed>();
        if (boxScript != null)
        {
            boxScript.ReactivateBox();
        }
        else
        {
            closedBox.SetActive(true);
        }

        Debug.Log("MysteryBoxOpen: Reset complete. Ready for next use.");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("MysteryBoxOpen: Player entered box range.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("MysteryBoxOpen: Player exited box range.");
        }
    }
}
