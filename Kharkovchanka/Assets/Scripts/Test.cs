using UnityEngine;

public class Test : MonoBehaviour
{
    public float deformationForce = 1f;
    public float deformationRadius = 2f;

    private MeshFilter meshFilter;
    private Mesh originalMesh;
    private Vector3[] originalVertices;

    void Start()
    {
        // Get the MeshFilter component and store the original mesh
        meshFilter = GetComponent<MeshFilter>();
        originalMesh = meshFilter.mesh;

        // Store the original vertices for deformation calculations
        originalVertices = originalMesh.vertices;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Deform the mesh based on the collision
        DeformMesh(collision.contacts[0].point);
    }

    void DeformMesh(Vector3 deformationPoint)
    {
        // Get the mesh vertices
        Vector3[] vertices = originalMesh.vertices;

        // Iterate through each vertex and calculate deformation based on distance from the impact point
        for (int i = 0; i < vertices.Length; i++)
        {
            float distance = Vector3.Distance(vertices[i], deformationPoint);

            // Apply deformation within the specified radius
            if (distance < deformationRadius)
            {
                // Calculate deformation factor based on distance
                float deformationFactor = 1 - (distance / deformationRadius);

                // Apply deformation along the vertex normal
                vertices[i] += originalMesh.normals[i] * deformationForce * deformationFactor;
            }
        }

        // Update the mesh with the deformed vertices
        meshFilter.mesh.SetVertices(vertices);
    }
}
