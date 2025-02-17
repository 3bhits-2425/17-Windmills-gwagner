using UnityEngine;

public class WindmillLockSpeedHelper : MonoBehaviour
{
    private float lockedSpeed = 0f;
    private bool isLocked = false;

    private WindmillDynamicSpeed windmillDynamicSpeed;

    private void Awake()
    {
        windmillDynamicSpeed = GetComponent<WindmillDynamicSpeed>();
        if (windmillDynamicSpeed == null)
        {
            Debug.LogError("WindmillDynamicSpeed component not found!");
        }
    }

    private void Update()
    {
        if (isLocked)
        {
            // Keep the windmill rotating at the locked speed
            transform.Rotate(Vector3.forward * lockedSpeed * Time.deltaTime);
        }
    }

    public void LockWindmillSpeed()
    {
        if (windmillDynamicSpeed != null)
        {
            lockedSpeed = windmillDynamicSpeed.GetNormalizedSpeed() * windmillDynamicSpeed.maxRotationSpeed;
            isLocked = true;

            // Optionally disable the WindmillDynamicSpeed script to prevent further speed adjustments
            windmillDynamicSpeed.enabled = false;

            Debug.Log($"Windmill speed locked at: {lockedSpeed}");
        }
    }

    public void UnlockWindmillSpeed()
    {
        isLocked = false;

        // Re-enable WindmillDynamicSpeed to allow normal speed adjustments
        if (windmillDynamicSpeed != null)
        {
            windmillDynamicSpeed.enabled = true;
        }

        Debug.Log("Windmill speed unlocked.");
    }
}
