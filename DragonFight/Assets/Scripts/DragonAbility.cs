using UnityEngine;

[System.Serializable]
public class DragonAbility
{
    public string abilityName;
    public int damage = 20;
    public float cooldown = 5f;
    public float attackRange = 5f;
    public GameObject vfxPrefab;
    public float animationTriggerDelay = 0.5f; // Used for Damage/VFX

    public AudioClip soundEffect;
    public float soundDelay = 0.5f; // NEW: Custom time for Sound!

    [HideInInspector] public float lastUsedTime;

    public bool IsReady()
    {
        return Time.time >= lastUsedTime + cooldown;
    }

    public void Use()
    {
        lastUsedTime = Time.time;
    }
}