using UnityEngine;
using System.Collections;

public class Pistol : MonoBehaviour
{
    [Header("Gun Stats")]
    public float damage = 20f;        // Damage dealt per shot
    public float range = 100f;        // How far the ray goes
    public float fireRate = 0.5f;     // Shots per second (lower = slower shots)
    public int maxAmmo = 12;          // Magazine size
    public float reloadTime = 1.5f;   // Seconds to reload

    private int currentAmmo;          // Tracks how much ammo is left
    private float nextTimeToFire = 0f; // Time until we can fire again
    private bool isReloading = false;

    [Header("References")]
    public Camera fpsCam;             // Main camera (assign in Inspector)
    public ParticleSystem muzzleFlash; // Optional muzzle flash effect
    public AudioSource gunAudio;      // Optional AudioSource for gunshot
    public AudioClip shootClip;       // Gunshot sound
    public AudioClip reloadClip;      // Reload sound

    void Start()
    {
        currentAmmo = maxAmmo; // Start full
    }

    void Update()
    {
        // If we’re in the middle of reloading, exit early
        if (isReloading) return;

        // Reload input (default "R" key)
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
            return;
        }

        // If out of ammo and player tries to shoot, reload automatically
        if (currentAmmo <= 0 && Input.GetButtonDown("Fire1"))
        {
            StartCoroutine(Reload());
            return;
        }

        // Fire input (default "Fire1" = left mouse button)
        if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate; // Limit firing rate
            Shoot();
        }
    }

    void Shoot()
    {
        currentAmmo--;

        // Play muzzle flash
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        // Play gunshot sound
        if (gunAudio != null && shootClip != null)
        {
            gunAudio.PlayOneShot(shootClip);
        }

        // Raycast from center of camera outward
        RaycastHit hit;
        if (fpsCam != null)
        {
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
            {
                // Debug to see what we hit
                Debug.Log("Pistol hit: " + hit.transform.name);

                // Check if target has a health component
                EnemyHealth enemy = hit.transform.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }
        else
        {
            Debug.LogWarning("No camera assigned to Pistol script!");
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        // Play reload sound
        if (gunAudio != null && reloadClip != null)
        {
            gunAudio.PlayOneShot(reloadClip);
        }

        // Wait reloadTime seconds
        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
    }
}
