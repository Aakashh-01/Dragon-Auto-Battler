using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownUI : MonoBehaviour
{
    [Header("Connections")]
    [SerializeField] private DragonAutoBattler playerDragon;
    [SerializeField] private Image cooldownOverlay; // The dark "Filled" image

    [Header("Which Ability is this?")]
    [Tooltip("Type 'fire', 'tail', or 'fly' (Exact match to Inspector name)")]
    [SerializeField] private string abilityName;

    private DragonAbility _linkedAbility;

    private void Start()
    {
        // Find the matching ability inside the Dragon script
        if (abilityName == "fire") _linkedAbility = playerDragon.fireAttack;
        else if (abilityName == "tail") _linkedAbility = playerDragon.tailAttack;
        else if (abilityName == "fly") _linkedAbility = playerDragon.flyAttack;

        if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0; // Start fresh
    }

    private void Update()
    {
        if (_linkedAbility == null || cooldownOverlay == null) return;

        // MATH: Calculate how much time is left
        float timeSinceUse = Time.time - _linkedAbility.lastUsedTime;
        float cooldownDuration = _linkedAbility.cooldown;

        if (timeSinceUse < cooldownDuration)
        {
            // We are on cooldown! Show the dark overlay.
            // 0.0 = Empty (Ready), 1.0 = Full (Just used)
            // We want it to shrink from 1 to 0.
            float percentRemaining = 1 - (timeSinceUse / cooldownDuration);
            cooldownOverlay.fillAmount = percentRemaining;
        }
        else
        {
            // Ready to use
            cooldownOverlay.fillAmount = 0;
        }
    }
}