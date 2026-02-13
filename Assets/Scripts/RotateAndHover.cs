using UnityEngine;

public class RotateAndHover : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Vector3 rotationAxis = Vector3.up; // Axis to rotate around
    public float rotationSpeed = 30f;         // Degrees per second

    [Header("Hover Settings")]
    public float hoverAmplitude = 0.5f;       // Max height offset
    public float hoverSpeed = 1f;             // Speed of hover
    public Vector3 hoverDirection = Vector3.up; // Hover axis

    private Vector3 localStartPos;

    void Start()
    {
        // Store local position relative to parent
        localStartPos = transform.localPosition;
    }

    void Update()
    {
        // --- Rotation ---
        transform.Rotate(rotationAxis.normalized * rotationSpeed * Time.deltaTime, Space.Self);

        // --- Hover ---
        float hoverOffset = Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;
        transform.localPosition = localStartPos + hoverDirection.normalized * hoverOffset;
    }
}