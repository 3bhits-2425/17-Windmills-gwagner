using UnityEngine;
using UnityEngine.UI;

public class WindmillDynamicSpeed : MonoBehaviour
{
    [SerializeField] private Light lampLight;
    [SerializeField] private float maxLightIntensity = 1f;
    [SerializeField] private Slider speedSlider;
    public float maxRotationSpeed = 300f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 30f;
    private float currentSpeed = 0f;
    private bool isLocked = false;

    private void Update()
    {
        if (!isLocked)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                currentSpeed += acceleration * Time.deltaTime;
            }
            else
            {
                currentSpeed -= deceleration * Time.deltaTime;
            }

            currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxRotationSpeed);

            if (speedSlider != null)
            {
                speedSlider.value = Mathf.Round(currentSpeed);
            }
        }

        // Windmill keeps rotating at the current speed, even when locked
        transform.Rotate(Vector3.forward * currentSpeed * Time.deltaTime);

        if (lampLight != null)
        {
            lampLight.intensity = Mathf.Lerp(0f, maxLightIntensity, currentSpeed / maxRotationSpeed);
        }
    }

    public float GetNormalizedSpeed()
    {
        return currentSpeed / maxRotationSpeed;
    }

    public void LockWindmillSpeed()
    {
        isLocked = true;
        if (speedSlider != null)
        {
            speedSlider.interactable = false;
        }
        Debug.Log($"Windmill speed locked at: {currentSpeed}");
    }

    public void UnlockWindmillSpeed()
    {
        isLocked = false;
        if (speedSlider != null)
        {
            speedSlider.interactable = true;
        }
        Debug.Log("Windmill speed unlocked.");
    }

    public void SetSpeed(float speed)
    {
        currentSpeed = Mathf.Clamp(speed, 0f, maxRotationSpeed);
        if (speedSlider != null)
        {
            speedSlider.value = Mathf.Round(currentSpeed);
        }
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
}
