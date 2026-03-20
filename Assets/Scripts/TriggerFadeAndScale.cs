using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerFadeAndScale : MonoBehaviour
{
    [Header("Object To Collapse (Z Only)")]
    public Transform objectToScaleDown;

    [Header("Parent Object To Grow (Children)")]
    public Transform objectToScaleUpParent;

    [Header("Timing")]
    public float duration = 1f;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        triggered = true;

        StartCoroutine(ScaleParentAndActivateChild());
    }

    IEnumerator ScaleParentAndActivateChild()
    {
        if (objectToScaleDown == null || objectToScaleUpParent == null)
            yield break;

        // --- Pick a random child ---
        int childCount = objectToScaleUpParent.childCount;
        if (childCount == 0) yield break;

        int randomIndex = Random.Range(0, childCount);

        // Deactivate all children first
        for (int i = 0; i < childCount; i++)
        {
            objectToScaleUpParent.GetChild(i).gameObject.SetActive(false);
        }

        // Activate only the random child
        objectToScaleUpParent.GetChild(randomIndex).gameObject.SetActive(true);

        // --- Capture starting scales ---
        Vector3 startScaleDown = objectToScaleDown.localScale;
        Vector3 startScaleUpParent = objectToScaleUpParent.localScale;
        Vector3 targetScaleUpParent = Vector3.one;

        float time = 0f;

        while (time < duration)
        {
            float t = Mathf.SmoothStep(0f, 1f, time / duration);

            // ----- Z Only Collapse -----
            float newZ = Mathf.Lerp(startScaleDown.z, 0f, t);
            objectToScaleDown.localScale = new Vector3(
                startScaleDown.x,
                startScaleDown.y,
                newZ
            );

            // ----- Parent Scale Up -----
            objectToScaleUpParent.localScale = Vector3.Lerp(startScaleUpParent, targetScaleUpParent, t);

            time += Time.deltaTime;
            yield return null;
        }

        // Ensure final values
        objectToScaleDown.localScale = new Vector3(startScaleDown.x, startScaleDown.y, 0f);
        objectToScaleUpParent.localScale = targetScaleUpParent;

        Destroy(objectToScaleDown.gameObject);
    }
}