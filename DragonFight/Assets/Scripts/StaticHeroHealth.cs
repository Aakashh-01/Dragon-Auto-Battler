using UnityEngine;
using UnityEngine.UI;

public class StaticHeroHealth : MonoBehaviour
{
    [Header("Connections")]
    [SerializeField] private CharacterStats playerStats;
    [SerializeField] private Image healthRingImage;

    private void Update()
    {
        if (playerStats != null && healthRingImage != null)
        {
            // Calculate health percentage (0.0 to 1.0)
            float fillPercent = (float)playerStats.CurrentHealth / playerStats.maxHealth;

            // Update the ring
            healthRingImage.fillAmount = fillPercent;

            // Optional: Change color based on health (Green -> Red)
            if (fillPercent > 0.5f)
                healthRingImage.color = Color.green;
            else if (fillPercent > 0.25f)
                healthRingImage.color = Color.yellow;
            else
                healthRingImage.color = Color.red;
        }
    }
}