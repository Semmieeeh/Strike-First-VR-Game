using UnityEngine;

public class MeshWarp : MonoBehaviour
{
    // Amplitude of the warp effect
    public float amplitude = 1.0f;

    // Frequency of the warp effect
    public float frequency = 1.0f;

    // Speed of the warp effect
    public float speed = 1.0f;

    // Reference to the object's mesh filter
    private MeshFilter meshFilter;

    void Start()
    {
        // Get the object's mesh filter
        meshFilter = GetComponent<MeshFilter>();
    }

    void Update()
    {
        // Get the object's mesh
        Mesh mesh = meshFilter.mesh;

        // Create a new array of vertex positions
        Vector3[] vertices = mesh.vertices;

        // Apply the warp effect to each vertex
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];

            // Calculate the amount of warp for this vertex
            float warp = Mathf.Sin(Time.time * speed + vertex.x * frequency + vertex.y * frequency + vertex.z * frequency) * amplitude;

            // Apply the warp to the vertex
            vertex.x += warp;
            vertex.y += warp;
            vertex.z += warp;

            // Update the vertex in the array
            vertices[i] = vertex;
        }

        // Assign the updated array of vertices to the mesh
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }
}