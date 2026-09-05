using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles player-initiated conversation with the caravan leader (e.g. via a world-space or
/// screen-space UI Button). Pauses the leader's waypoint movement and has it face the player
/// for a short time, then hands control back to CaravanLeaderPath.
/// </summary>
[RequireComponent(typeof(CaravanLeaderPath))]
public class CaravanLeaderInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [Tooltip("UI Button that starts the conversation — wire this in the Inspector, or leave empty and call StartTalking() from your own dialogue/interaction system.")]
    public Button talkButton;
    [Tooltip("Assign the player so the leader knows who to face during dialogue.")]
    public Transform playerTransform;
    [Tooltip("How long (seconds) the leader stays in talking mode before returning to caravan movement.")]
    public float dialogueTime = 3f;
    [Tooltip("Degrees per second the leader turns to face the player while talking.")]
    public float faceTurnSpeed = 360f;

    private CaravanLeaderPath leaderPath;
    private Animator animator;

    private bool isTalking = false;
    private float talkTimer = 0f;

    void Start()
    {
        leaderPath = GetComponent<CaravanLeaderPath>();
        animator = leaderPath.animator;

        if (talkButton != null)
        {
            talkButton.onClick.AddListener(StartTalking);
        }
    }

    void Update()
    {
        if (!isTalking) return;

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

        // Count down dialogue time, then return to caravan movement
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
        if (leaderPath.CurrentState == CaravanLeaderPath.LeaderState.Talking) return;

        isTalking = true;
        talkTimer = dialogueTime;

        leaderPath.EnterTalkingState();

        if (animator != null)
        {
            animator.SetBool("is_walking", false);
            animator.SetBool("is_speaking", true);
        }
    }

    /// <summary>
    /// Ends talking mode early if you ever need to call it manually.
    /// </summary>
    public void StopTalking()
    {
        isTalking = false;

        if (animator != null)
            animator.SetBool("is_speaking", false);

        leaderPath.ExitTalkingStateToMoving();
    }
}