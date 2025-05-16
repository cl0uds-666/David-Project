using UnityEngine;
using System.Collections;
using TMPro;

public class AssaultRifle : MonoBehaviour
{
    [Header("Gun Stats")]
    public float damage = 15f;
    public float range = 100f;
    public int maxAmmo = 30;
    public int maxReserveAmmo = 120;
    public float reloadTime = 2.0f;
    public float fireRate = 10f; // bullets per second

    private int currentAmmo;
    private int currentReserveAmmo;
    private bool isReloading = false;
    private float nextTimeToFire = 0f;

    [Header("References")]
    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public AudioSource gunAudio;
    public AudioClip shootClip;
    public AudioClip reloadClip;

    [Header("Reload Animation")]
    public Transform modelTransform;
    public Transform reloadPosition;
    public Transform defaultPosition;
    public float reloadMoveSpeed = 5f;
    public Transform reloadRotation;
    public Transform defaultRotation;
    public float reloadRotateSpeed = 5f;

    [Header("Aiming")]
    public Transform aimPosition;
    public Transform aimRotation;
    public float aimMoveSpeed = 10f;
    public float aimRotateSpeed = 10f;

    private bool isAiming = false;

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

        HandleAiming();

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

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && currentAmmo > 0)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }

        // Stop looped gun audio if fire released or no ammo
        if (Input.GetButtonUp("Fire1") || currentAmmo <= 0)
        {
            if (gunAudio.isPlaying && gunAudio.clip == shootClip)
            {
                gunAudio.Stop();
                gunAudio.loop = false;
            }
        }
    }
    void Shoot()
    {
        if (currentAmmo <= 0) return;

        currentAmmo--;

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
            StartCoroutine(StopMuzzleFlash());
        }

        WFX_LightFlicker lightFlicker = GetComponentInChildren<WFX_LightFlicker>();
        if (lightFlicker != null)
        {
            lightFlicker.StartFlicker();
        }

        if (gunAudio != null && shootClip != null && !gunAudio.isPlaying)
        {
            gunAudio.clip = shootClip;
            gunAudio.loop = true;
            gunAudio.Play();
        }

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
        else
        {
            Debug.LogWarning("No camera assigned to AssaultRifle script!");
        }

        UpdateAmmoUI();
    }


    IEnumerator StopMuzzleFlash()
    {
        yield return new WaitForSeconds(0.1f);
        muzzleFlash.Stop();
    }

    IEnumerator Reload()
    {
        if (currentReserveAmmo <= 0 || currentAmmo == maxAmmo) yield break;

        isReloading = true;
        Debug.Log("Reloading...");

        if (gunAudio.isPlaying && gunAudio.clip == shootClip)
        {
            gunAudio.Stop();
            gunAudio.loop = false;
        }

        if (gunAudio != null && reloadClip != null)
        {
            gunAudio.PlayOneShot(reloadClip);
        }

        yield return StartCoroutine(MoveGun(modelTransform, reloadPosition.localPosition, reloadRotation.localRotation));
        yield return new WaitForSeconds(reloadTime);

        int ammoNeeded = maxAmmo - currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, currentReserveAmmo);

        currentAmmo += ammoToReload;
        currentReserveAmmo -= ammoToReload;

        yield return StartCoroutine(MoveGun(modelTransform, defaultPosition.localPosition, defaultRotation.localRotation));

        isReloading = false;
        UpdateAmmoUI();
    }

    IEnumerator MoveGun(Transform obj, Vector3 targetLocalPos, Quaternion targetLocalRot)
    {
        while (Vector3.Distance(obj.localPosition, targetLocalPos) > 0.01f || Quaternion.Angle(obj.localRotation, targetLocalRot) > 0.1f)
        {
            obj.localPosition = Vector3.Lerp(obj.localPosition, targetLocalPos, reloadMoveSpeed * Time.deltaTime);
            obj.localRotation = Quaternion.Lerp(obj.localRotation, targetLocalRot, reloadRotateSpeed * Time.deltaTime);
            yield return null;
        }

        obj.localPosition = targetLocalPos;
        obj.localRotation = targetLocalRot;
    }

    void HandleAiming()
    {
        if (Input.GetButton("Fire2"))
        {
            isAiming = true;
        }
        else
        {
            isAiming = false;
        }

        if (modelTransform != null)
        {
            if (isAiming)
            {
                modelTransform.localPosition = Vector3.Lerp(modelTransform.localPosition, aimPosition.localPosition, aimMoveSpeed * Time.deltaTime);
                modelTransform.localRotation = Quaternion.Lerp(modelTransform.localRotation, aimRotation.localRotation, aimRotateSpeed * Time.deltaTime);
            }
            else
            {
                modelTransform.localPosition = Vector3.Lerp(modelTransform.localPosition, defaultPosition.localPosition, aimMoveSpeed * Time.deltaTime);
                modelTransform.localRotation = Quaternion.Lerp(modelTransform.localRotation, defaultRotation.localRotation, aimRotateSpeed * Time.deltaTime);
            }
        }
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = $"{currentAmmo} / {currentReserveAmmo}";
        }
    }
}
