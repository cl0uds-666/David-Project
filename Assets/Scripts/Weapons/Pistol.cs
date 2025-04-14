using UnityEngine;
using System.Collections;
using TMPro;

public class Pistol : MonoBehaviour
{
    [Header("Gun Stats")]
    public float damage = 20f;
    public float range = 100f;
    public int maxAmmo = 8;
    public int maxReserveAmmo = 56;
    public float reloadTime = 1.5f;

    private int currentAmmo;
    private int currentReserveAmmo;
    private bool isReloading = false;

    [Header("References")]
    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public AudioSource gunAudio;
    public AudioClip shootClip;
    public AudioClip reloadClip;

    [Header("Reload Animation")]
    public Transform modelTransform;       // Assign the gun model here
    public Transform reloadPosition;       // Position to dip to
    public Transform defaultPosition;      // Normal gun position
    public float reloadMoveSpeed = 5f;     // Speed of lerping
    
    [Header("Reload Rotation")]
    public Transform reloadRotation;      // Target reload rotation
    public Transform defaultRotation;     // Normal resting rotation
    public float reloadRotateSpeed = 5f;  // Speed of rotation lerp


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

        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
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

        if (gunAudio != null && shootClip != null)
        {
            gunAudio.PlayOneShot(shootClip);
        }

        RaycastHit hit;
        if (fpsCam != null)
        {
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
            {
                EnemyHealth enemy = hit.transform.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    PlayerPoints playerPoints = FindObjectOfType<PlayerPoints>();
                    if (playerPoints != null)
                    {
                        enemy.TakeDamage(damage, playerPoints);
                    }
                    else
                    {
                        Debug.LogWarning("PlayerPoints system not found!");
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("No camera assigned to Pistol script!");
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

        if (gunAudio != null && reloadClip != null)
        {
            gunAudio.PlayOneShot(reloadClip);
        }

        // Move to reload position + rotate
        yield return StartCoroutine(MoveGun(modelTransform, reloadPosition.localPosition, reloadRotation.localRotation));

        yield return new WaitForSeconds(reloadTime);

        int ammoNeeded = maxAmmo - currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, currentReserveAmmo);

        currentAmmo += ammoToReload;
        currentReserveAmmo -= ammoToReload;

        // Move back to default position + rotate back
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


    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = $"{currentAmmo} / {currentReserveAmmo}";
        }
    }
}
