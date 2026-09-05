using UnityEngine;

public class SunController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform levelStart;
    [SerializeField] private Transform levelEnd;

    [Header("Sun Rotation")]
    [Tooltip("Sun rotation angle at the start of the level")]
    public float startAngle = 30f;

    [Tooltip("Sun rotation angle at the end of the level")]
    public float endAngle = 120f;

    [Header("Sun")]
    [SerializeField] private Transform sun;

    // Furthest progress the player has reached.
    // This prevents the sun from moving backwards.
    private float maxProgress = 0f;


    private void Update()
    {
        float currentProgress = GetPlayerProgress();

        // Sun can only move from START -> END.
        maxProgress = Mathf.Max(maxProgress, currentProgress);

        UpdateSun();
    }


    private float GetPlayerProgress()
    {
        UnityEngine.Vector3 start = levelStart.position;
        UnityEngine.Vector3 end = levelEnd.position;

        // Ignore height.
        start.y = 0f;
        end.y = 0f;

        // Direction from start of level to end of level.
        UnityEngine.Vector3 levelDirection = (end - start).normalized;

        // Player position relative to level start.
        UnityEngine.Vector3 playerRelative = player.position - start;
        playerRelative.y = 0f;

        // Project player's position onto the level direction.
        float distanceAlongLevel =
            UnityEngine.Vector3.Dot(playerRelative, levelDirection);

        // Total level length.
        float levelLength = UnityEngine.Vector3.Distance(start, end);

        // Convert to 0 -> 1.
        float progress = distanceAlongLevel / levelLength;

        return Mathf.Clamp01(progress);
    }


    private void UpdateSun()
    {
        // Linearly interpolate between the start and end angles.
        float currentAngle = Mathf.Lerp(
            startAngle,
            endAngle,
            maxProgress
        );

        // Set the directional light's X rotation.
        sun.rotation = Quaternion.Euler(
            currentAngle,
            sun.rotation.eulerAngles.y,
            sun.rotation.eulerAngles.z
        );
    }
}