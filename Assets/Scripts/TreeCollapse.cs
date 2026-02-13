using UnityEngine;

public class TreeCollapse : MonoBehaviour
{
    [Header("Collapse Settings")]
    [Range(0, 1)]
    public float t;

    public float finalScale = 0.001f;          // Final scale at t = 1
    public float heightPull = 1.2f;            // How much it moves down
    public AnimationCurve suckCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Rotation Settings")]
    public Vector3 rotationAxis = Vector3.up;  // Axis to rotate around
    public float maxRotationAngle = 360f;      // Total rotation over collapse
    public AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector3 initialScale;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Awake()
    {
        initialScale = transform.localScale;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void Update()
    {
        // --- Evaluate collapse progress ---
        float s = suckCurve.Evaluate(t);

        // --- SCALE ---
        float scale = Mathf.Lerp(1f, finalScale, s);
        transform.localScale = initialScale * scale;

        // --- POSITION OFFSET (downward) ---
        transform.position = initialPosition - Vector3.up * s * heightPull;

        // --- LIMITED ROTATION ---
        float rot = rotationCurve.Evaluate(t) * maxRotationAngle; // total rotation based on t
        transform.rotation = initialRotation * Quaternion.AngleAxis(rot, rotationAxis);
    }
}