using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Attach this to a hand (or a fingertip child transform under the hand) to let the player
/// draw on any MapSurfaceDrawing surface just by pointing at it and holding the trigger.
///
/// SETUP:
/// - Set 'Tip' to the transform that should act as the drawing point (fingertip bone if you
///   have one rigged, otherwise the hand's own transform works fine to start).
/// - Set 'Hand Node' to LeftHand or RightHand depending on which hand this is on.
/// - Tune 'Tip Ray Length' so it reaches just past the fingertip - long enough to detect
///   contact with the map surface, short enough that it doesn't draw through the air.
///
/// PC TESTING (no headset): hold the configured test key while your simulated hand
/// (via AutoHandPlayerControllerInputSimulator, Q/E + mouse) is near the map surface.
/// </summary>
public class HandDrawTool : MonoBehaviour
{
    [Header("Tip")]
    public Transform tip;                 // fingertip or hand transform, points forward at the surface
    public float tipRayLength = 0.03f;    // how far past the tip counts as "touching"

    [Header("XR Trigger")]
    public XRNode handNode = XRNode.RightHand;
    [Range(0f, 1f)] public float triggerThreshold = 0.5f;

    [Header("PC Test Fallback")]
    public bool allowPcKeyTesting = true;
    public KeyCode pcTestDrawKey = KeyCode.Space; // separate from the simulator's grab key (Mouse0)

    void Update()
    {
        if (tip == null) return;
        if (!IsDrawInputHeld()) return;

        // First check what's actually in front of the tip so we don't rely on assumptions
        // about which map is nearby - works with any number of MapSurfaceDrawing objects in the scene.
        if (Physics.Raycast(tip.position, tip.forward, out RaycastHit hit, tipRayLength))
        {
            MapSurfaceDrawing surface = hit.collider.GetComponent<MapSurfaceDrawing>();
            if (surface != null)
                surface.TryDrawAtWorldPoint(tip.position, tip.forward, tipRayLength);
        }
    }

    bool IsDrawInputHeld()
    {
        if (IsVRActive())
        {
            var device = InputDevices.GetDeviceAtXRNode(handNode);
            if (device.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
                return triggerValue >= triggerThreshold;
            return false;
        }
        else
        {
            return allowPcKeyTesting && Input.GetKey(pcTestDrawKey);
        }
    }

    bool IsVRActive()
    {
        var loader = UnityEngine.XR.Management.XRGeneralSettings.Instance?.Manager?.activeLoader;
        return loader != null && !loader.name.Contains("Mock");
    }

    void OnDrawGizmosSelected()
    {
        // Visualize the draw ray in the Scene view so you can see exactly where it'll register contact
        if (tip == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(tip.position, tip.position + tip.forward * tipRayLength);
    }
}
