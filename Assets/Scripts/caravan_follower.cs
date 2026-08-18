using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CaravanFollower : MonoBehaviour
{
    [Header("Leader to Follow")]
    public Transform leaderTransform;

    [Header("Formation Offset")]
    [Tooltip("X = Right/Left distance relative to leader, Z = Behind/Ahead distance")]
    public Vector2 formationOffset; 

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        // Match speed to leader, but allow slight variation to prevent robotic synchronized stepping
        agent.speed = 1.0f + Random.Range(-0.05f, 0.05f);
        
        // Prevent members from bumping each other out of line
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
    }

    void Update()
    {
        if (leaderTransform == null) return;

        // Calculate world space coordinate based on the leader's current position and rotation direction
        Vector3 targetPosition = leaderTransform.position 
                               + (leaderTransform.right * formationOffset.x) 
                               + (leaderTransform.forward * formationOffset.y)
                               ;

        // Tell this individual agent to walk toward their assigned spot in the formation
        agent.SetDestination(targetPosition);
    }
}