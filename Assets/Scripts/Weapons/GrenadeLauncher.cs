using UnityEngine;
using System.Collections;
using TMPro;

public class GrenadeLauncher : MonoBehaviour
{
    [Header("Ammo Settings")]
    public int maxAmmo = 1;
    public int maxReserveAmmo = 5;
    public float reloadTime = 2f;

    private int currentAmmo;
    private int currentReserveAmmo;
    private bool isReloading = false;

    [Header("Launch Settings")]
    public GameObject grenadePrefab;
    public Transform firePoint;
    public float launchForce = 700f;

    [Header("FX")]
    public ParticleSystem muzzleFlash;
    public AudioSource gunAudio;
    public AudioClip shootClip;
    public AudioClip reloadClip;

    [Header("UI")]
    public TextMeshProUGUI ammoText;

    void Start()
    {
        currentAmmo = maxAmmo;
        currentReserveAmmo = maxReserveAmmo;
        UpdateAmmoUI();
    }

    void Update()
    {
        if (isReloading) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
            return;
        }

        if (currentAmmo <= 0 && Input.GetButtonDown("Fire1"))
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButtonDown("Fire1") && currentAmmo > 0)
        {
            Fire();
        }
    }

    void Fire()
    {
        currentAmmo--;

        // Instantiate and launch grenade
        if (grenadePrefab != null && firePoint != null)
        {
            GameObject grenade = Instantiate(grenadePrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = grenade.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(firePoint.forward * launchForce);
            }
        }

        if (muzzleFlash != null)
            muzzleFlash.Play();

        if (gunAudio != null && shootClip != null)
            gunAudio.PlayOneShot(shootClip);

        UpdateAmmoUI();
    }

    IEnumerator Reload()
    {
        if (currentReserveAmmo <= 0 || currentAmmo == maxAmmo) yield break;

        isReloading = true;

        if (gunAudio != null && reloadClip != null)
            gunAudio.PlayOneShot(reloadClip);

        yield return new WaitForSeconds(reloadTime);

        int ammoNeeded = maxAmmo - currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, currentReserveAmmo);

        currentAmmo += ammoToReload;
        currentReserveAmmo -= ammoToReload;

        isReloading = false;
        UpdateAmmoUI();
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
            ammoText.text = $"{currentAmmo} / {currentReserveAmmo}";
    }
}
