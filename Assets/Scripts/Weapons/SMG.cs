using UnityEngine;
using System.Collections;
using TMPro;

public class SMG : MonoBehaviour
{
    [Header("Gun Stats")]
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 0.1f;       // Time between shots (0.1 = 10 shots/sec)
    public int maxAmmo = 30;
    public int maxReserveAmmo = 120;
    public float reloadTime = 1.2f;

    private int currentAmmo;
    private int currentReserveAmmo;
    private bool isReloading = false;
    private bool isFiring = false;
    private float nextTimeToFire = 0f;

    [Header("References")]
    public Camera fpsCam;
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

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && currentAmmo > 0)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }

        if (Input.GetButtonDown("Fire1") && currentAmmo <= 0 && currentReserveAmmo > 0)
        {
            StartCoroutine(Reload());
        }
    }
    void Shoot()
    {
        currentAmmo--;

        if (muzzleFlash != null)
            muzzleFlash.Play();

        if (gunAudio != null && shootClip != null)
            gunAudio.PlayOneShot(shootClip);

        if (fpsCam != null)
        {
            Ray ray = new Ray(fpsCam.transform.position, fpsCam.transform.forward);
            RaycastHit[] hits = Physics.RaycastAll(ray, range);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            float currentDamage = damage;
            int maxPenetration = 5;

            foreach (RaycastHit hitInfo in hits)
            {
                if (maxPenetration <= 0) break;

                EnemyHealth enemy = hitInfo.transform.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    PlayerPoints playerPoints = FindObjectOfType<PlayerPoints>();
                    enemy.TakeDamage(currentDamage, playerPoints);
                    currentDamage *= 0.75f;
                    maxPenetration--;
                }
            }
        }

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
