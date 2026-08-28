using UnityEngine;

public class VRStaminaManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private PlayerStatusSystem statusSystem;

    [Header("Locomotion Tracking Target")]
    [Tooltip("Drag the AutoHandPlayer GameObject here.")]
    [SerializeField] private Transform autoHandPlayer;

    [Header("Sprint Thresholds & Costs")]
    [Tooltip("Movement speed (m/s) required to begin draining stamina.")]
    public float sprintSpeedThreshold = 3.5f;
    [Tooltip("Stamina drained per second while sprinting.")]
    public float sprintDrainRate = 15f;

    [Header("Stamina Recovery Settings")]
    public float maxStamina = 100f;
    public float regenRate = 20f;       // Stamina per second restored when resting
    public float regenDelay = 1.5f;     // Seconds to wait after sprinting before regen begins

    // Internal tracking values
    private Vector3 prevPlayerPos;
    private float lastSprintTime;
    public float currentStamina;

    void Start()
    {
        if (statusSystem == null) 
            statusSystem = GetComponent<PlayerStatusSystem>();

        currentStamina = maxStamina;

        if (autoHandPlayer != null)
        {
            prevPlayerPos = autoHandPlayer.position;
        }
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;
        if (deltaTime <= 0f || autoHandPlayer == null) return;

        // 1. Calculate Locomotion Speed (m/s)
        Vector3 displacement = autoHandPlayer.position - prevPlayerPos;
        prevPlayerPos = autoHandPlayer.position;
        float currentSpeed = displacement.magnitude / deltaTime;

        // 2. Track Sprinting Exertion
        bool isSprinting = currentSpeed >= sprintSpeedThreshold;
        float staminaRecoveredThisFrame = 0f;

        if (isSprinting)
        {
            // Drain stamina while sprinting
            float drainAmount = sprintDrainRate * deltaTime;
            currentStamina = Mathf.Max(0f, currentStamina - drainAmount);
            lastSprintTime = Time.time; // Reset the delay timer
        }
        else if (Time.time >= lastSprintTime + regenDelay && currentStamina < maxStamina)
        {
            // Recover stamina after delay
            float potentialRegen = regenRate * deltaTime;
            float newStamina = Mathf.Min(maxStamina, currentStamina + potentialRegen);
            staminaRecoveredThisFrame = newStamina - currentStamina;
            currentStamina = newStamina;
        }

        // 3. Sync state back to the PlayerStatusSystem
        statusSystem.stamina = currentStamina;
        statusSystem.TickStatusSystem(deltaTime, staminaRecoveredThisFrame);
    }
}