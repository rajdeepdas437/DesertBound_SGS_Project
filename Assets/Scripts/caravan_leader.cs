using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events; // Needed for UnityEvent

[RequireComponent(typeof(NavMeshAgent))]
public class CaravanLeaderPath : MonoBehaviour
{
    [Header("Route Setup")]
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;

    [Header("Speed Settings")]
    public float walkSpeed = 1.0f;

    [Header("Events")]
    public int stormTriggerWaypointIndex = 2; // Waypoint 3 is index 2 (0, 1, 2)
    public UnityEvent OnSandstormStart;

    private NavMeshAgent agent;
    private bool stormTriggered = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;

        SetNextDestination();
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 1.5f)
        {
            // Check if we arrived at Waypoint 3 (index 2)
            if (currentWaypointIndex == stormTriggerWaypointIndex && !stormTriggered)
            {
                stormTriggered = true;
                OnSandstormStart?.Invoke(); // Fire the sandstorm event!
            }

            // Move to next waypoint (or stop)
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            SetNextDestination();
        }
    }

    void SetNextDestination()
    {
        if (waypoints.Length > 0 && waypoints[currentWaypointIndex] != null)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }
}