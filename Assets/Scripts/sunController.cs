using System.Numerics;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SunController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform levelStart;
    [SerializeField] private Transform levelEnd;

    [Header("Sun Offsets")]
    [Tooltip("Sun position relative to player at 09:00 (East)")]
    public float eastSunx = 500f;
    [Tooltip("Sun position relative to player at 16:00 (West)")]
    public float westSunx = -500f;

    public float sunDistance = 509.9f;

    [Header("Sun")]
    [SerializeField] private Transform sunSphere;

    // Furthest progress the player has reached.
    // This prevents the sun from moving backwards.
    private float maxProgress = 0f;


    private void Update()
    {
        float currentProgress = GetPlayerProgress();

        // Sun can only move EAST -> WEST.
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
        // Calculate the angles of the East and West positions.
        float eastAngle = Mathf.Atan2(
            Mathf.Sqrt(sunDistance * sunDistance - eastSunx * eastSunx),
            eastSunx
        );

        float westAngle = Mathf.Atan2(
            Mathf.Sqrt(sunDistance * sunDistance - westSunx * westSunx),
            westSunx
        );

        // Linearly interpolate the ANGLE, not X/Y.
        float currentAngle = Mathf.Lerp(
            eastAngle,
            westAngle,
            maxProgress
        );

        // Convert angle back into X/Y coordinates.
        float x = Mathf.Cos(currentAngle) * sunDistance;
        float y = Mathf.Sin(currentAngle) * sunDistance;

        UnityEngine.Vector3 sunOffset = new UnityEngine.Vector3(x, y, 0f);

        // Position relative to player.
        sunSphere.position = player.position + sunOffset;
    }
}