using UnityEngine;

public class VRStaminaManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private PlayerStatusSystem statusSystem;

    [Header("Tracked Transforms (Drag from Hierarchy)")]
    [SerializeField] private Transform rootPlayer;     // AutoHandPlayer
    [SerializeField] private Transform headCamera;     // Camera (head)
    [SerializeField] private Transform leftController; // Controller (left)
    [SerializeField] private Transform rightController;// Controller (right)

    [Header("Physical Movement Thresholds")]
    [Tooltip("Min speed (m/s) for root movement to count as exertion.")]
    public float walkThreshold = 1.2f;
    [Tooltip("Speed (m/s) considered full sprint exertion.")]
    public float sprintThreshold = 3.5f;
    [Tooltip("Min hand velocity (m/s) to count as active work/swinging.")]
    public float handExertionThreshold = 1.5f;

    [Header("Stamina Costs & Recovery Rates")]
    public float maxStamina = 100f;
    public float baseSprintDrain = 15f;    // Stamina per second
    public float heavySwingDrain = 10f;     // Per high-speed swing threshold
    public float regenRate = 20f;           // Base stamina per second restored
    public float regenDelay = 1.5f;         // Delay in seconds before regen starts

    // Internal tracking positions & state
    private Vector3 prevRootPos, prevHeadPos, prevLeftPos, prevRightPos;
    private float lastExertionTime;
    private float currentStamina;

    void Start()
    {
        if (statusSystem == null) statusSystem = GetComponent<PlayerStatusSystem>();
        currentStamina = maxStamina;

        // Initialize tracking positions
        if (rootPlayer) prevRootPos = rootPlayer.position;
        if (headCamera) prevHeadPos = headCamera.position;
        if (leftController) prevLeftPos = leftController.position;
        if (rightController) prevRightPos = rightController.position;
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;
        if (deltaTime <= 0f) return;

        // 1. Calculate Real-Time Velocities (m/s)
        float rootSpeed = GetVelocity(rootPlayer, ref prevRootPos, deltaTime);
        float headSpeed = GetVelocity(headCamera, ref prevHeadPos, deltaTime);
        float leftHandSpeed = GetVelocity(leftController, ref prevLeftPos, deltaTime);
        float rightHandSpeed = GetVelocity(rightController, ref prevRightPos, deltaTime);

        float totalDrain = 0f;

        // 2. Body Locomotion Exertion (Walking/Sprinting)
        if (rootSpeed > walkThreshold)
        {
            float locomotionFactor = Mathf.InverseLerp(walkThreshold, sprintThreshold, rootSpeed);
            totalDrain += baseSprintDrain * locomotionFactor * deltaTime;
        }

        // 3. Physical Hand Exertion (Swinging, Chopping, Mining)
        float maxHandSpeed = Mathf.Max(leftHandSpeed, rightHandSpeed);
        if (maxHandSpeed > handExertionThreshold)
        {
            // Drain scales dynamically with how fast hands move above the threshold
            totalDrain += (maxHandSpeed - handExertionThreshold) * 5f * deltaTime;
        }

        // 4. Apply Drain or Trigger Regeneration
        float staminaRecoveredThisFrame = 0f;

        if (totalDrain > 0f)
        {
            currentStamina = Mathf.Clamp(currentStamina - totalDrain, 0f, maxStamina);
            lastExertionTime = Time.time; // Reset regen timer
        }
        else if (Time.time >= lastExertionTime + regenDelay && currentStamina < maxStamina)
        {
            // Regenerate Stamina
            float potentialRegen = regenRate * deltaTime;
            float newStamina = Mathf.Min(maxStamina, currentStamina + potentialRegen);
            staminaRecoveredThisFrame = newStamina - currentStamina;
            currentStamina = newStamina;
        }

        // 5. Sync state back to the PlayerStatusSystem
        statusSystem.stamina = currentStamina;
        statusSystem.TickStatusSystem(deltaTime, staminaRecoveredThisFrame);
    }
    public float get_stamina()
    {
        return currentStamina;
    }
    private float GetVelocity(Transform target, ref Vector3 previousPosition, float deltaTime)
    {
        if (target == null) return 0f;
        Vector3 displacement = target.position - previousPosition;
        previousPosition = target.position;
        return displacement.magnitude / deltaTime;
    }
}