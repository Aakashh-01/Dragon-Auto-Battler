using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [Header("Connections")]
    [SerializeField] private CharacterStats myStats; // The logic
    [SerializeField] private Image healthFillImage;  // The green/red bar

    [Header("Settings")]
    [SerializeField] private bool lookAtCamera = true; // Make it always face the screen

    private Camera _mainCam;

    private void Start()
    {
        _mainCam = Camera.main;

        // Safety check: Auto-find stats if not assigned
        if (myStats == null)
            myStats = GetComponentInParent<CharacterStats>();
    }

    private void LateUpdate()
    {
        // 1. Update the Bar Visuals
        if (myStats != null && healthFillImage != null)
        {
            // Calculate percentage (Current / Max)
            float fillPercent = (float)myStats.CurrentHealth / myStats.maxHealth;
            healthFillImage.fillAmount = fillPercent;
        }

        // 2. Billboarding (Face the Camera)
        // This ensures the health bar doesn't look flat or rotate with the dragon
        if (lookAtCamera && _mainCam != null)
        {
            transform.LookAt(transform.position + _mainCam.transform.rotation * Vector3.forward,
                             _mainCam.transform.rotation * Vector3.up);
        }
    }
}