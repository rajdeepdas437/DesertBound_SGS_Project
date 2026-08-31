using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Attach to the ROOT grabbable object (the one with Rigidbody + Grabbable on it).
/// Switches between a closed visual (e.g. rolled cylinder) and an open visual
/// (e.g. flat plane with MapSurfaceDrawing) by toggling which child is active.
///
/// The root object is what the player actually holds - it never changes when the
/// map opens/closes, so there's no re-grab, no physics change, just a visual swap.
/// </summary>
public class MapOpenClose : MonoBehaviour
{
    [Header("Visuals")]
    public GameObject closedVisual;   // e.g. the rolled cylinder mesh
    public GameObject openVisual;     // e.g. the flat plane with MapSurfaceDrawing

    [Header("Toggle Input")]
    [Tooltip("Which controller's button opens/closes the map - typically the hand NOT currently holding it.")]
    public XRNode toggleHandNode = XRNode.LeftHand;
    public KeyCode pcTestToggleKey = KeyCode.O;

    private bool wasPressed = false;

    void Start()
    {
        SetOpen(false); // always start closed
    }

    void Update()
    {
        bool pressed = IsTogglePressed();

        if (pressed && !wasPressed)
            ToggleOpen();

        wasPressed = pressed;
    }

    public void ToggleOpen()
    {
        bool currentlyOpen = openVisual != null && openVisual.activeSelf;
        SetOpen(!currentlyOpen);
    }

    public void SetOpen(bool open)
    {
        if (closedVisual != null) closedVisual.SetActive(!open);
        if (openVisual != null) openVisual.SetActive(open);
    }

    bool IsTogglePressed()
    {
        if (IsVRActive())
        {
            var device = InputDevices.GetDeviceAtXRNode(toggleHandNode);
            if (device.TryGetFeatureValue(CommonUsages.primaryButton, out bool primary))
                return primary;
            return false;
        }
        else
        {
            return Input.GetKey(pcTestToggleKey);
        }
    }

    bool IsVRActive()
    {
        var loader = UnityEngine.XR.Management.XRGeneralSettings.Instance?.Manager?.activeLoader;
        return loader != null && !loader.name.Contains("Mock");
    }
}