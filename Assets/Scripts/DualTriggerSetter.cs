using UnityEngine;

public class DualTriggerLerpT_OneShot : MonoBehaviour
{
    [Header("Objects that must enter together")]
    public GameObject objectA;
    public GameObject objectB;

    [Header("Target Script")]
    public TreeCollapse treeCollapse;

    [Header("Lerp Settings")]
    public float lerpDuration = 1.5f;

    private bool insideA;
    private bool insideB;
    private bool triggered;

    private float startT;
    private float timer;

    void Start()
    {
        if (treeCollapse != null)
            startT = treeCollapse.t;
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (other.gameObject == objectA) insideA = true;
        if (other.gameObject == objectB) insideB = true;

        if (insideA && insideB)
        {
            Debug.Log("DoubleTriggered");
            triggered = true;
            timer = 0f;

            if (treeCollapse != null)
                startT = treeCollapse.t;
        }
    }

    void Update()
    {
        if (!triggered || treeCollapse == null)
            return;

        timer += Time.deltaTime;
        float t01 = Mathf.Clamp01(timer / lerpDuration);

        treeCollapse.t = Mathf.Lerp(startT, 1f, t01);
    }
}
