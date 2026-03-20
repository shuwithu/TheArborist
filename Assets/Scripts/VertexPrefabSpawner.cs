using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class VertexPrefabSpawner : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject prefab;
    public int spawnCount = 10;

    [Header("Scale Settings")]
    public float minScale = 0.5f;
    public float maxScale = 1.5f;

    [Header("Rotation")]
    public bool randomYRotation = true;

    [Header("Space")]
    public bool useWorldSpace = true;

    private Mesh mesh;
    private Vector3[] vertices;

    void Start()
    {
        SpawnPrefabs();
    }

    public void SpawnPrefabs()
    {
        if (prefab == null) return;

        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;

        if (vertices.Length == 0) return;

        for (int i = 0; i < spawnCount; i++)
        {
            // Pick random vertex
            int randomIndex = Random.Range(0, vertices.Length);
            Vector3 vertexPosition = vertices[randomIndex];

            // Convert to world space if needed
            if (useWorldSpace)
                vertexPosition = transform.TransformPoint(vertexPosition);

            GameObject obj = Instantiate(prefab, vertexPosition, Quaternion.identity);

            // Random scale
            float randomScale = Random.Range(minScale, maxScale);
            obj.transform.localScale = Vector3.one * randomScale;

            // Optional random Y rotation
            if (randomYRotation)
                obj.transform.Rotate(0f, Random.Range(0f, 360f), 0f);

            // Parent to this object (optional, keeps hierarchy clean)
            obj.transform.SetParent(transform);
        }
    }
}