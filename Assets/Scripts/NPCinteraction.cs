using UnityEngine;
using UnityEngine.XR;
using Autohand;

public class NPCInteraction : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private GameObject promptCanvas;
    [SerializeField] private Transform vrCameraTransform;

    [Header("Auto Hand Settings")]
    [Tooltip("Optional: Will automatically find AutoHandPlayer if left empty")]
    [SerializeField] private AutoHandPlayer autoHandPlayer;

    private bool isPlayerInRadius = false;

    private void Start()
    {
        // Find main camera if not assigned
        if (vrCameraTransform == null && Camera.main != null)
        {
            vrCameraTransform = Camera.main.transform;
        }

        // Find player automatically if unassigned
        if (autoHandPlayer == null)
        {
            autoHandPlayer = FindFirstObjectByType<AutoHandPlayer>();
        }

        // Ensure canvas starts hidden
        if (promptCanvas != null)
        {
            promptCanvas.SetActive(false);
        }
    }

    private void Update()
    {
        // Make prompt face the player's VR headset
        if (isPlayerInRadius && promptCanvas != null && promptCanvas.activeSelf && vrCameraTransform != null)
        {
            promptCanvas.transform.LookAt(promptCanvas.transform.position + vrCameraTransform.rotation * Vector3.forward,
                vrCameraTransform.rotation * Vector3.up);
        }

        // Check for A/X button press when player is in radius
        if (isPlayerInRadius && CheckPrimaryButtonPressed())
        {
            StartDialogue();
        }
    }

    private bool CheckPrimaryButtonPressed()
    {
        // Primary Button = A on Right Controller / X on Left Controller
        return IsButtonPressedOnDevice(XRNode.RightHand, CommonUsages.primaryButton) ||
               IsButtonPressedOnDevice(XRNode.LeftHand, CommonUsages.primaryButton);
    }

    private bool IsButtonPressedOnDevice(XRNode node, InputFeatureUsage<bool> button)
    {
        var device = InputDevices.GetDeviceAtXRNode(node);
        if (device.isValid && device.TryGetFeatureValue(button, out bool isPressed))
        {
            return isPressed;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<AutoHandPlayer>() != null || other.CompareTag("Player"))
        {
            isPlayerInRadius = true;
            if (promptCanvas != null) promptCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<AutoHandPlayer>() != null || other.CompareTag("Player"))
        {
            isPlayerInRadius = false;
            if (promptCanvas != null) promptCanvas.SetActive(false);
        }
    }

    private void StartDialogue()
    {
        Debug.Log("Dialogue Started with NPC!");

        // Hide prompt during conversation
        if (promptCanvas != null) promptCanvas.SetActive(false);

        // TODO: Trigger your dialogue UI system or audio here
    }
}