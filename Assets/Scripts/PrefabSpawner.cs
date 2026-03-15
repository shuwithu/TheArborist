using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PrefabSpawner : MonoBehaviour
{
    [Header("Pool Settings")]
    public GameObject[] prefabsToSpawn; // Replaced single prefab with array
    public int totalToSpawn = 4;        // Set to 4 as requested

    [Header("Randomization Settings")]
    public Vector2 scaleRange = new Vector2(0.05f, 0.2f);
    public Vector2 rotationRange = new Vector2(0f, 360f);

    [Header("Persistence Settings")]
    public bool makePersistent = true;
    public bool clearExistingOnSpawn = true;

    private List<GameObject> spawnedPrefabs = new List<GameObject>();

    void Start()
    {
        if (clearExistingOnSpawn)
        {
            ClearExistingSpawnedPrefabs();
        }
        SpawnPrefabsAtUniqueVertices();
    }

    void SpawnPrefabsAtUniqueVertices()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null || prefabsToSpawn == null || prefabsToSpawn.Length == 0)
        {
            Debug.LogWarning("MeshFilter missing or prefabsToSpawn array is empty.");
            return;
        }

        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;

        // Pick ONE prefab from the array to use for all 4 spawns
        GameObject selectedPrefab = prefabsToSpawn[Random.Range(0, prefabsToSpawn.Length)];

        // Get 4 random unique vertex indices
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < vertices.Length; i++) availableIndices.Add(i);

        int actualSpawnCount = Mathf.Min(totalToSpawn, vertices.Length);

        for (int i = 0; i < actualSpawnCount; i++)
        {
            int listIndex = Random.Range(0, availableIndices.Count);
            int vertexIndex = availableIndices[listIndex];
            availableIndices.RemoveAt(listIndex);

            Vector3 worldPos = transform.TransformPoint(vertices[vertexIndex]);

            Quaternion randomRotation = Quaternion.Euler(
                Random.Range(rotationRange.x, rotationRange.y),
                Random.Range(rotationRange.x, rotationRange.y),
                Random.Range(rotationRange.x, rotationRange.y)
            );

            float randomScale = Random.Range(scaleRange.x, scaleRange.y);

            // CHANGED: Added 'this.transform' as the parent so it appears underneath
            GameObject spawned = Instantiate(selectedPrefab, worldPos, randomRotation, this.transform);
            spawned.transform.localScale = Vector3.one * randomScale;

            if (makePersistent)
            {
                MakePersistent(spawned);
            }

            spawnedPrefabs.Add(spawned);
        }
    }

    void MakePersistent(GameObject obj)
    {
        // HIDDEN FLAGS REMOVED: Objects will now be fully visible in hierarchy
        if (!Application.isPlaying)
        {
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(obj, "Spawn Persistent Prefab");
#endif
        }

        if (obj.GetComponent<PersistentSpawnedObject>() == null)
        {
            obj.AddComponent<PersistentSpawnedObject>();
        }
    }

    void ClearExistingSpawnedPrefabs()
    {
        // Search children specifically to find objects to clean up
        PersistentSpawnedObject[] existingObjects = GetComponentsInChildren<PersistentSpawnedObject>();
        for (int i = existingObjects.Length - 1; i >= 0; i--)
        {
            if (existingObjects[i] != null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    Undo.DestroyObjectImmediate(existingObjects[i].gameObject);
                else
                    Destroy(existingObjects[i].gameObject);
#else
                Destroy(existingObjects[i].gameObject);
#endif
            }
        }
        spawnedPrefabs.Clear();
    }

    [ContextMenu("Spawn Prefabs (Editor Only)")]
    void SpawnPrefabsInEditor()
    {
#if UNITY_EDITOR
        if (Application.isPlaying) return;
        if (clearExistingOnSpawn) ClearExistingSpawnedPrefabs();
        SpawnPrefabsAtUniqueVertices();
        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
#endif
    }

    [ContextMenu("Clear Spawned Prefabs")]
    void ClearSpawnedPrefabs()
    {
        ClearExistingSpawnedPrefabs();
    }
}

public class PersistentSpawnedObject : MonoBehaviour { }