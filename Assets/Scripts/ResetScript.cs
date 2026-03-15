using UnityEngine;

public class ResetScript : MonoBehaviour
{
    [System.Serializable]
    public class ResetEntry
    {
        public GameObject instanceInScene; // The object currently in the world
        public GameObject originalPrefab;  // The clean asset from your Project folder

        [HideInInspector] public Vector3 checkpointPos;
        [HideInInspector] public Quaternion checkpointRot;
    }

    public ResetEntry[] objectsToReset;

    void Awake()
    {
        // On very first start, set Position 0 as the default checkpoint
        SetCheckpoints();
    }

    void Update()
    {
        // 1. PRESS 'C' TO LOCK IN "POSITION 1"
        if (Input.GetKeyDown(KeyCode.C))
        {
            SetCheckpoints();
        }

        // 2. PRESS 'R' TO RETURN TO THE LAST 'C' POSITION
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetToLastCheckpoint();
        }
    }

    public void SetCheckpoints()
    {
        foreach (var entry in objectsToReset)
        {
            if (entry.instanceInScene != null)
            {
                entry.checkpointPos = entry.instanceInScene.transform.position;
                entry.checkpointRot = entry.instanceInScene.transform.rotation;
            }
        }
        Debug.Log("<color=yellow>Checkpoints Saved!</color> Press 'R' to return here.");
    }

    public void ResetToLastCheckpoint()
    {
        Debug.Log("<color=green>Resetting to Checkpoints...</color>");

        for (int i = 0; i < objectsToReset.Length; i++)
        {
            ResetEntry entry = objectsToReset[i];

            if (entry.originalPrefab != null)
            {
                // Destroy the current instance (even if it's at Position 2 or 3)
                if (entry.instanceInScene != null)
                {
                    Destroy(entry.instanceInScene);
                }

                // Spawn the fresh prefab at the "C" key location (Position 1)
                GameObject newClone = Instantiate(entry.originalPrefab, entry.checkpointPos, entry.checkpointRot);

                // Re-link so we can do it again
                entry.instanceInScene = newClone;
            }
        }
    }
}