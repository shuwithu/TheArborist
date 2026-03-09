using UnityEngine;

public class SimpleRotationLock : MonoBehaviour
{
    [Header("Freeze Settings")]
    public bool freezeX = true;
    public bool freezeY = false;
    public bool freezeZ = true;

    private float _lockedX;
    private float _lockedY;
    private float _lockedZ;

    void Awake()
    {
        // Capture whatever the rotation is in the editor/scene at start
        Vector3 startRotation = transform.eulerAngles;
        _lockedX = startRotation.x;
        _lockedY = startRotation.y;
        _lockedZ = startRotation.z;
    }

    void LateUpdate()
    {
        Vector3 currentRotation = transform.eulerAngles;

        // If frozen, use the 'start' value. If not, use the current (hand) value.
        float x = freezeX ? _lockedX : currentRotation.x;
        float y = freezeY ? _lockedY : currentRotation.y;
        float z = freezeZ ? _lockedZ : currentRotation.z;

        transform.rotation = Quaternion.Euler(x, y, z);
    }
}