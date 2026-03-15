using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class RandomChildDeleter : MonoBehaviour
{
    [Tooltip("How many children should remain after the purge?")]
    public int targetCount = 100;

    // This adds a command when you right-click the component header in the Inspector
    [ContextMenu("Execute Random Deletion")]
    public void DeleteRandomChildren()
    {
        int childCount = transform.childCount;

        if (childCount <= targetCount)
        {
            Debug.LogWarning("Already at or below target count.");
            return;
        }

        // Gather children
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
        }

        int amountToDelete = childCount - targetCount;

        // Undo-compatible deletion
        for (int i = 0; i < amountToDelete; i++)
        {
            int randomIndex = Random.Range(0, children.Count);
            GameObject objToDestroy = children[randomIndex];
            children.RemoveAt(randomIndex);

            Undo.DestroyObjectImmediate(objToDestroy);
        }

        Debug.Log($"Successfully reduced to {targetCount} children.");
    }
}