using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CaravanLeaderPath : MonoBehaviour
{
    [Header("Route Setup")]
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;

    [Header("Speed Settings")]
    public float walkSpeed = 1.0f; // Very slow, march pace

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;

        // Start marching toward the first waypoint
        SetNextDestination();
    }

    void Update()
    {
        // Check if the leader has reached the current waypoint
        if (!agent.pathPending && agent.remainingDistance < 1.5f)
        {
            // Move to the next waypoint in the list
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