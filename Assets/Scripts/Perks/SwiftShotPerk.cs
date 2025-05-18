using UnityEngine;

public class SwiftShotPerk : MonoBehaviour
{
    public float reloadSpeedMultiplier = 0.5f;  // 50% faster reload
    public float switchSpeedMultiplier = 0.7f;  // 30% faster weapon switch
    public float sprintSpeedMultiplier = 1.25f; // 25% faster sprint

    public static bool IsActive { get; private set; } = false;

    public static float ReloadMultiplier => IsActive ? 0.5f : 1f;
    public static float SwitchMultiplier => IsActive ? 0.7f : 1f;
    public static float SprintMultiplier => IsActive ? 1.25f : 1f;

    public static void ActivatePerk()
    {
        IsActive = true;
    }
}
