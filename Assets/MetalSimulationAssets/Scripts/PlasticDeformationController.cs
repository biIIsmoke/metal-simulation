using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlasticDeformationController : MonoBehaviour
{
    [SerializeField] private ComputeShader plasticDeformationShader;
    [SerializeField] private Mesh mesh;
    [SerializeField] private Vector3[] vertices;
    
    public Vector3 impactPoint;
    public Vector3 impactVector;
    

    private void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        vertices = mesh.vertices;
    }
    
    public void OnGPUCall()
    {
        int vector3Size = sizeof(float) * 3;
        int totalSize = vector3Size;
        
        ComputeBuffer vertexBuffer = new ComputeBuffer(vertices.Length, totalSize);
        vertexBuffer.SetData(vertices);
        
        plasticDeformationShader.SetBuffer(0,"vertices", vertexBuffer);
        //plasticDeformationShader.SetFloat("resolution", vertices.Length);
        plasticDeformationShader.SetVector("impactPoint", impactPoint);
        plasticDeformationShader.SetVector("impactVector", impactVector);
        
        plasticDeformationShader.Dispatch(0, vertices.Length/10,1,1);
        
        vertexBuffer.GetData(vertices);
        
        //do the mesh update
        
        mesh.SetVertices(vertices);
        
        vertexBuffer.Dispose();
        
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 50), "GPU Call"))
        {
            OnGPUCall();
        }
    }
}
