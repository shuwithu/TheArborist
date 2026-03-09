using System.Collections;
using UnityEngine;

public class ScaleandSpawn : MonoBehaviour
{
    [Header("Trigger Filter")]
    [Tooltip("Only objects with this tag will be consumed")]
    public string triggerTag = "tree";

    [Header("Spawn")]
    public GameObject prefabToSpawn;

    [Header("Spawn Offset")]
    [Tooltip("Vertical offset from this object's origin")]
    public float verticalOffset = 0f;

    [Header("Timing")]
    public float scaleDownTime = 0.4f;
    public float scaleUpTime = 0.4f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(triggerTag)) return;

        StartCoroutine(ConsumeAndSpawn(other.gameObject));
    }

    IEnumerator ConsumeAndSpawn(GameObject target)
    {
        Transform self = transform;
        Transform t = target.transform;
        Vector3 startScale = t.localScale;

        // Scale Object B down
        float timer = 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime / scaleDownTime;
            if (t != null)
                t.localScale = Vector3.Lerp(startScale, Vector3.zero, timer);
            yield return null;
        }

        Destroy(target);

        // Spawn position = self origin + vertical offset
        Vector3 spawnPosition = self.position + Vector3.up * verticalOffset;

        GameObject spawned = Instantiate(
            prefabToSpawn,
            spawnPosition,
            self.rotation
        );

        spawned.transform.localScale = Vector3.zero;

        // Scale prefab up
        timer = 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime / scaleUpTime;
            spawned.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, timer);
            yield return null;
        }

        spawned.transform.localScale = Vector3.one;
    }
}