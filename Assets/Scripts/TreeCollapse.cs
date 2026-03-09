using UnityEngine;
using Oculus.Interaction;

public class TreeCollapse : MonoBehaviour
{
    [Header("Scale Settings")]
    [Range(0, 1)]
    public float t;
    public float finalScale = 0.001f;
    public AnimationCurve suckCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Collider Settings")]
    public BoxCollider treeCollider;
    public Vector3 collapsedSize = new Vector3(0.1f, 0.1f, 0.1f);
    public Vector3 collapsedCenter = Vector3.zero;

    private Vector3 initialScale;
    private Vector3 initialColliderSize;
    private Vector3 initialColliderCenter;

    void Awake()
    {
        initialScale = transform.localScale;

        if (treeCollider == null)
            treeCollider = GetComponent<BoxCollider>();

        if (treeCollider != null)
        {
            initialColliderSize = treeCollider.size;
            initialColliderCenter = treeCollider.center;
        }
    }

    // LateUpdate is the "Secret Sauce" — it runs after the GrabTransformer
    void LateUpdate()
    {
        float s = suckCurve.Evaluate(t);

        // 1. Force the Transform Scale
        float scaleMultiplier = Mathf.Lerp(1f, finalScale, s);
        transform.localScale = initialScale * scaleMultiplier;

        // 2. Force the Collider Dimensions
        if (treeCollider != null)
        {
            treeCollider.size = Vector3.Lerp(initialColliderSize, collapsedSize, s);
            treeCollider.center = Vector3.Lerp(initialColliderCenter, collapsedCenter, s);
        }
    }
}