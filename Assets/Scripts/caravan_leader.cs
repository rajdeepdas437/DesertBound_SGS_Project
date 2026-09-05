using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events; // Needed for UnityEvent

[RequireComponent(typeof(NavMeshAgent))]
public class CaravanLeaderPath : MonoBehaviour
{
    public enum LeaderState { Moving, Waiting, Talking }

    [Header("Route Setup")]
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;

    [Header("Speed Settings")]
    public float walkSpeed = 1.0f;

    [Header("Events")]
    public int stormTriggerWaypointIndex = 2; // Waypoint 3 is index 2 (0, 1, 2)
    public UnityEvent OnSandstormStart;

    [Header("Waiting State")]
    [Tooltip("Player transform used to check proximity before the caravan resumes moving.")]
    public Transform playerTransform;
    [Tooltip("Radius the player must enter for the caravan to resume moving after stopping at a waypoint.")]
    public float waitRadius = 10f;

    [Header("Animation")]
    [Tooltip("Animator driving the is_walking / is_speaking bools.")]
    public Animator animator;

    private NavMeshAgent agent;
    private bool stormTriggered = false;
    [SerializeField] private ParticleSystem sandParticleSystem;

    private bool isStormActive = false;
    [Header("Sandstorm Skybox")]
    [SerializeField] private Material sandstormSkybox;
    private Material originalSkybox;

    /// <summary>Current high-level state of the leader. Followers read this to sync their own waiting behaviour.</summary>
    public LeaderState CurrentState { get; private set; } = LeaderState.Moving;

    /// <summary>True while the leader is stopped at a waypoint waiting for the player to catch up.</summary>
    public bool IsWaiting => CurrentState == LeaderState.Waiting;

    public void StartSandstorm()
    {
        Debug.Log("THE STORM HAS BEGUN!");

        isStormActive = true;

        if (sandParticleSystem != null)
        {
            sandParticleSystem.Play();
        }

        // Change skybox
        if (sandstormSkybox != null)
        {
            RenderSettings.skybox = sandstormSkybox;
            DynamicGI.UpdateEnvironment();
        }
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;

        SetNextDestination();
        originalSkybox = RenderSettings.skybox;
    }

    void Update()
    {
        switch (CurrentState)
        {
            case LeaderState.Moving:
                HandleMoving();
                break;
            case LeaderState.Waiting:
                HandleWaiting();
                break;
            case LeaderState.Talking:
                // Movement/animation while talking is driven by CaravanLeaderInteraction.
                break;
        }
    }

    private void HandleMoving()
    {
        if (animator != null)
            animator.SetBool("is_walking", true);

        if (!agent.pathPending && agent.remainingDistance < 1.5f)
        {
            // Check if we arrived at Waypoint 3 (index 2)
            if (currentWaypointIndex == stormTriggerWaypointIndex && !stormTriggered)
            {
                stormTriggered = true;
                OnSandstormStart?.Invoke(); // Fire the sandstorm event!
            }

            EnterWaitingState();
        }
    }

    private void EnterWaitingState()
    {
        CurrentState = LeaderState.Waiting;

        agent.isStopped = true;

        if (animator != null)
        {
            animator.SetBool("is_walking", false);
            animator.SetBool("is_speaking", true); // leader "speaks"/idles while the caravan waits
        }
    }

    private void HandleWaiting()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (distance <= waitRadius)
        {
            ExitWaitingState();
        }
    }

    private void ExitWaitingState()
    {
        CurrentState = LeaderState.Moving;

        if (animator != null)
            animator.SetBool("is_speaking", false);

        agent.isStopped = false;

        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        SetNextDestination();
    }

    // ---------- Called by CaravanLeaderInteraction ----------

    /// <summary>Pauses waypoint movement so the leader can face/talk to the player.</summary>
    public void EnterTalkingState()
    {
        CurrentState = LeaderState.Talking;
        agent.isStopped = true;
    }

    /// <summary>Returns the leader to normal caravan movement after a conversation ends.</summary>
    public void ExitTalkingStateToMoving()
    {
        CurrentState = LeaderState.Moving;
        agent.isStopped = false;
    }

    void SetNextDestination()
    {
        if (waypoints.Length > 0 && waypoints[currentWaypointIndex] != null)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }
}