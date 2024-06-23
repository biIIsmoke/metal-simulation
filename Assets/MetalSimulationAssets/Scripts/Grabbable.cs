using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    [SerializeField] private ComputeShader grabShader;
    [SerializeField] private Vector3[] vertices; 
    [SerializeField] private Vector3[] defaultVertices;
    [SerializeField] private Vector3 grabPosition;
    [SerializeField] private Vector3 currentPosition;
    [SerializeField] private float grabRadius;
    
    [Range(0,1)]
    [SerializeField] private float maxDeform = 0.2f;
    [Range(0,1)]
    [SerializeField] private float maxElasticity = 0.1f;
    
    
    private MeshFilter meshFilter;
    private ComputeBuffer vertexBuffer;
    private ComputeBuffer defaultVertexBuffer;
    
    // Start is called before the first frame update
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        vertices = meshFilter.mesh.vertices;
        defaultVertices = meshFilter.mesh.vertices;
        
        //sends default vertex data to be used in calculations without change
        int vector3Size = sizeof(float) * 3;
        int totalSize = vector3Size;
        defaultVertexBuffer = new ComputeBuffer(defaultVertices.Length, totalSize);
        defaultVertexBuffer.SetData(defaultVertices);
        grabShader.SetBuffer(0,"defaultVertices", defaultVertexBuffer);
        defaultVertexBuffer.Dispose();
    }

    public void CallShader(Vector3 newCurrentPosition, float grabSize)
    {
        //currentPosition = newCurrentPosition;
        int vector3Size = sizeof(float) * 3;
        int totalSize = vector3Size;
        
        vertexBuffer = new ComputeBuffer(vertices.Length, totalSize);
        vertexBuffer.SetData(vertices);
        
        //set constraints
        grabShader.SetVector("grabPosition", grabPosition);
        grabShader.SetVector("currentPosition", currentPosition);
        grabShader.SetFloat("grabSize", grabSize);
        grabShader.SetFloat("maxElasticity", maxElasticity);
        grabShader.SetFloat("maxDeform", maxDeform);
        
        grabShader.SetBuffer(0,"vertices", vertexBuffer);
        
        grabShader.Dispatch(0, vertices.Length/512,1,1);
        
        vertexBuffer.GetData(vertices);
        
        
        //apply changes
        UpdateMesh();
        vertexBuffer.Dispose();
    }
    
    private void UpdateMesh()
    {
        meshFilter.mesh.vertices = vertices;
        //meshFilter.mesh.RecalculateBounds();
        //meshFilter.mesh.RecalculateNormals();
        //meshFilter.mesh.RecalculateTangents();
    }

    public void SetGrabPosition(Vector3 position)
    {
        grabPosition = position;
    }
}
