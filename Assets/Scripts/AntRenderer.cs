using UnityEngine;

public class AntRenderer : MonoBehaviour
{
    public AntColony colony;
    public Material antMaterial;

    public float scaleValue = 0.5f;
    
    private Mesh quadMesh;
    private Matrix4x4[] matrices;

    void Start()
    {
        CreateQuadMesh();
        
        if (colony == null)
        {
            Debug.LogWarning("AntRenderer: colony is null");
            enabled = false;
            return;
        }
        
        matrices = new Matrix4x4[colony.ants.Length];
    }

    void Update()
    {
        for (int i = 0; i < colony.ants.Length; i++)
        {
            Ant ant = colony.ants[i];
            
            Vector3 scale = Vector3.one * scaleValue;
            
            Quaternion rotation = Quaternion.Euler(0, 0, ant.angle * Mathf.Rad2Deg);
            
            matrices[i] = Matrix4x4.TRS(new Vector3(ant.position.x, ant.position.y, 0), rotation, scale);
        }
        
        Graphics.DrawMeshInstanced(quadMesh, 0, antMaterial, matrices);
    }

    private void CreateQuadMesh()
    {
        quadMesh = new Mesh();

        Vector3[] vertices =
        {
            new Vector3(-0.5f, -0.5f, 0.0f),
            new Vector3(0.5f, -0.5f, 0.0f),
            new Vector3(-0.5f, 0.5f, 0.0f),
            new Vector3(0.5f, 0.5f, 0.0f),
        };

        int[] triangles = { 0, 2, 1, 2, 3, 1 };

        Vector2[] uv =
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
        };
        
        quadMesh.vertices = vertices;
        quadMesh.triangles = triangles;
        quadMesh.uv = uv;
        quadMesh.RecalculateNormals();
    }
}
