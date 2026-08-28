using TMPro;
using UnityEngine;

public class PlayerStatusSystem : MonoBehaviour
{
    [Header("Core Parameters (0 - 100)")]
    [Range(0, 100)] public float health = 100f;
    [Range(0, 100)] public float hunger = 100f;
    [Range(0, 100)] public float stamina = 100f;
    [Range(0, 100)] public float water = 100f;

    [Header("Day / Night Cycle Settings")]
    [Tooltip("Check this true during Night mode to lower expense rates.")]
    public bool isNight = false;
    [Tooltip("Multiplier applied to passive drain at night (0.6 = 60% of day rate).")]
    public float nightMultiplier = 0.6f;

    [Header("Passive Depletion Rates (Per Second)")]
    public float baseWaterDrain = 0.050f;
    public float baseHungerDrain = 0.025f;

    [Header("Stamina Costs")]
    [Tooltip("Amount of Water consumed per 1 point of recovered Stamina.")]
    public float waterCostPerStamina = 0.02f;
    [Tooltip("Amount of Hunger consumed per 1 point of recovered Stamina.")]
    public float hungerCostPerStamina = 0.01f;
    public float nightWaterCostMultiplier = 0.8f;

    [Header("Health Drain Penalties (Per Second)")]
    public float zeroWaterHealthDrain = 1.5f;
    public float zeroHungerHealthDrain = 0.5f;
    public TextMeshProUGUI uivalues;
    private const float MAX_STAT = 100f;

    void Update()
    {
        (float health, float hunger, float water) = GetStatusValues();
        uivalues.text = $"{(int)health}\n{(int)stamina}\n{(int)water}\n{(int)hunger}";
        
    }

    /// <summary>
    /// Processes passive depletion, stamina recovery costs, and starvation/dehydration damage.
    /// </summary>
    public void TickStatusSystem(float deltaTime, float staminaRecovered)
    {
        // 1. Determine Day/Night Multiplier
        float modeMultiplier = isNight ? nightMultiplier : 1.0f;

        // 2. Passive Drain
        float waterDrain = baseWaterDrain * modeMultiplier * deltaTime;
        float hungerDrain = baseHungerDrain * modeMultiplier * deltaTime;

        // 3. Stamina Recovery Costs (Paid via Water & Hunger)
        float currentNightWaterMult = isNight ? nightWaterCostMultiplier : 1.0f;
        float waterStaminaCost = staminaRecovered * waterCostPerStamina * currentNightWaterMult;
        float hungerStaminaCost = staminaRecovered * hungerCostPerStamina;

        // 4. Apply Depletions & Clamp Values [0, 100]
        water = Mathf.Clamp(water - (waterDrain + waterStaminaCost), 0f, MAX_STAT);
        hunger = Mathf.Clamp(hunger - (hungerDrain + hungerStaminaCost), 0f, MAX_STAT);

        // 5. Health Drain Logic for Depleted Reserves
        float totalHealthDrain = 0f;

        if (water <= 0f)
        {
            totalHealthDrain += zeroWaterHealthDrain;
        }

        if (hunger <= 0f)
        {
            totalHealthDrain += zeroHungerHealthDrain;
        }

        health = Mathf.Clamp(health - (totalHealthDrain * deltaTime), 0f, MAX_STAT);
    }

    /// <summary>
    /// Helper method to fetch final values cleanly.
    /// </summary>
    public (float health, float hunger, float water) GetStatusValues()
    {
        return (
            Mathf.Round(health * 100f) / 100f,
            Mathf.Round(hunger * 100f) / 100f,
            Mathf.Round(water * 100f) / 100f
        );
    }
}