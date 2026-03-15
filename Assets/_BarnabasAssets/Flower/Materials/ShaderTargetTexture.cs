using UnityEngine;

[ExecuteInEditMode]
public class ShaderTargetTexture : MonoBehaviour
{
    public Transform targetObject;
    private Material mat;

    void Start()
    {
        Renderer r = GetComponent<Renderer>();
        if (r != null) mat = r.sharedMaterial;
    }

    void Update()
    {
        if (mat != null && targetObject != null)
        {
            // Sends the world position of the target to the shader
            mat.SetVector("_TargetPos", targetObject.position);
        }
    }
}