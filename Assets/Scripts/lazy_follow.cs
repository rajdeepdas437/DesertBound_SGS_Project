using UnityEngine;

public class LazyFollow : MonoBehaviour
{
    [Header("Target & Offset")]
    [SerializeField] private Transform targetCamera;
    [SerializeField] private float distance = 1.5f;

    [Header("Movement Settings")]
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float rotateSpeed = 5f;

    [Header("Deadzones")]
    [SerializeField] private float angleThreshold = 25f; // Deg before rotation updates
    [SerializeField] private float distanceThreshold = 0.3f; // Distance before position updates

    private void Start()
    {
        if (targetCamera == null && Camera.main != null)
        {
            targetCamera = Camera.main.transform;
        }
    }

    private void LateUpdate()
    {
        if (targetCamera == null) return;

        // Calculate target position in front of camera (flattened to keep horizon level)
        Vector3 cameraForward = Vector3.ProjectOnPlane(targetCamera.forward, Vector3.up).normalized;
        Vector3 targetPosition = targetCamera.position + (cameraForward * distance);

        // Only update position if target moved beyond distance threshold
        if (Vector3.Distance(transform.position, targetPosition) > distanceThreshold)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
        }

        // Only rotate toward camera if head turned past angle threshold
        Vector3 directionToCamera = transform.position - targetCamera.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToCamera, Vector3.up);

        if (Quaternion.Angle(transform.rotation, targetRotation) > angleThreshold)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
        }
    }
}