using UnityEngine;

public class ResetScript : MonoBehaviour
{
    [System.Serializable]
    public class ResetEntry
    {
        public GameObject instanceInScene; // The one currently being moved/changed
        public GameObject originalPrefab;  // The "Clean" file from your Project window

        // No longer using [HideInInspector] startPos here 
        // because we want to grab it in real-time.
    }

    public ResetEntry[] objectsToReset;

    void Awake()
    {
        // We can keep the log to know the system is ready
        Debug.Log("Reset System Ready. Press 'R' to swap for fresh prefabs in place.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetAll();
        }
    }

    public void ResetAll()
    {
        Debug.Log("R Pressed: Replacing objects at current locations...");

        for (int i = 0; i < objectsToReset.Length; i++)
        {
            ResetEntry entry = objectsToReset[i];

            if (entry.instanceInScene != null && entry.originalPrefab != null)
            {
                // 1. Capture the position RIGHT NOW
                Vector3 currentPos = entry.instanceInScene.transform.position;
                // Capture current rotation too, so it doesn't rotate back to default
                Quaternion currentRot = entry.instanceInScene.transform.rotation;

                // 2. Delete the "Dirty" object
                Destroy(entry.instanceInScene);

                // 3. Spawn the "Clean" prefab at the CURRENT location
                GameObject newClone = Instantiate(entry.originalPrefab, currentPos, currentRot);

                // 4. Re-link the new clone so the next 'R' press knows which object to kill
                entry.instanceInScene = newClone;
            }
        }
    }
}