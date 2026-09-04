using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class CaravanFollower : MonoBehaviour
{
    [Header("Leader to Follow")]
    public Transform leaderTransform;

    [Header("Formation Offset")]
    [Tooltip("X = Right/Left distance relative to leader, Z = Behind/Ahead distance")]
    public Vector2 formationOffset;

    [Header("Talking Mode")]
    [Tooltip("Assign the player so the NPC knows who to face during dialogue.")]
    public Transform playerTransform;
    [Tooltip("UI Button that starts the dialogue — wire this in the Inspector, or leave empty and call StartTalking() from your own dialogue system.")]
    public Button talkButton;
    [Tooltip("How long (seconds) the NPC stays in talking mode before returning to caravan movement.")]
    public float dialogueTime = 3f;
    [Tooltip("Degrees per second the NPC turns to face the player while talking.")]
    public float faceTurnSpeed = 360f;

    public Animator animator;
    private NavMeshAgent agent;

    private bool isTalking = false;
    private float talkTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Match speed to leader, but allow slight variation to prevent robotic synchronized stepping
        agent.speed = 1.0f + Random.Range(-0.05f, 0.05f);

        // Prevent members from bumping each other out of line
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;

        // Wire the button automatically if one was assigned in the Inspector
        if (talkButton != null)
        {
            talkButton.onClick.AddListener(StartTalking);
        }
    }

    void Update()
    {
        if (isTalking)
        {
            HandleTalking();
        }
        else
        {
            HandleCaravanMovement();
        }
    }

    // ---------- Mode 1: Normal NavMesh caravan following ----------
    private void HandleCaravanMovement()
    {
        if (leaderTransform == null) return;

        // Calculate world space coordinate based on the leader's current position and rotation direction
        Vector3 targetPosition = leaderTransform.position
                               + (leaderTransform.right * formationOffset.x)
                               + (leaderTransform.forward * formationOffset.y);

        // Tell this individual agent to walk toward their assigned spot in the formation
        agent.SetDestination(targetPosition);

        if (animator != null)
            animator.SetBool("is_walking", true);
    }

    // ---------- Mode 2: Talking mode ----------
    private void HandleTalking()
    {
        // Face the player smoothly
        if (playerTransform != null)
        {
            Vector3 direction = playerTransform.position - transform.position;
            direction.y = 0f; // keep upright, only rotate on Y axis

            if (direction.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    faceTurnSpeed * Time.deltaTime
                );
            }
        }

        // Count down dialogue time, then return to caravan mode
        talkTimer -= Time.deltaTime;
        if (talkTimer <= 0f)
        {
            StopTalking();
        }
    }

    /// <summary>
    /// Call this from the UI Button's OnClick(), or from your own dialogue system.
    /// </summary>
    public void StartTalking()
    {
        isTalking = true;
        talkTimer = dialogueTime;

        agent.isStopped = true;
        agent.ResetPath(); // clear current path so it doesn't resume mid-route unexpectedly

        if (animator != null)
            animator.SetBool("is_walking", false);
    }

    /// <summary>
    /// Ends talking mode early if you ever need to call it manually.
    /// </summary>
    public void StopTalking()
    {
        isTalking = false;
        agent.isStopped = false;
    }
}