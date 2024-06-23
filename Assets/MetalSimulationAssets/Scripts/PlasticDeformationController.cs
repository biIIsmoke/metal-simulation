using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlasticDeformationController : MonoBehaviour
{
    [SerializeField] private ComputeShader plasticDeformationShader;
    [SerializeField] private MeshCollider meshCollider;
    [SerializeField] private Vector3[] vertices; 
    [SerializeField] private Vector3[] defaultVertices;

    [Range(0,10)]
    [SerializeField] private float deformRadius = 0.2f;
    [Range(0,10)]
    [SerializeField] private float maxDeform = 0.1f;
    [Range(0,1)]
    [SerializeField] private float damageFalloff = 1.0f;
    [Range(0,10)]
    [SerializeField] private float damageMultiplier = 1.0f;
    [Range(0,100000)] //this will be hardness???
    public float minDamage = 1.0f;
    
    
    public bool useGPU = false;
    public Vector3 impactPoint;
    
    private MeshFilter meshFilter;
    private ComputeBuffer vertexBuffer;
    private ComputeBuffer defaultVertexBuffer;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        vertices = meshFilter.mesh.vertices;
        defaultVertices = meshFilter.mesh.vertices;
        
        //sends default vertex data to be used in calculations without change
        int vector3Size = sizeof(float) * 3;
        int totalSize = vector3Size;
        defaultVertexBuffer = new ComputeBuffer(defaultVertices.Length, totalSize);
        defaultVertexBuffer.SetData(defaultVertices);
        plasticDeformationShader.SetBuffer(0,"defaultVertices", defaultVertexBuffer);
        defaultVertexBuffer.Dispose();
    }

    private void OnCollisionEnter(Collision other)
    {
        float collisionPower = other.impulse.magnitude;
        
        //Debug.Log(collisionPower);
        
        if (collisionPower > minDamage)
        {
            foreach (ContactPoint contactPoint in other.contacts)
            {
                impactPoint = transform.InverseTransformPoint(contactPoint.point);
                
                if (useGPU)
                {
                    OnGPUCall();
                }
                else
                {
                    OnCPUCall();
                }
            }
        }
    }

    private void OnGPUCall()
    {
        int vector3Size = sizeof(float) * 3;
        int totalSize = vector3Size;
        
        vertexBuffer = new ComputeBuffer(vertices.Length, totalSize);
        vertexBuffer.SetData(vertices);
        
        //set constraints
        plasticDeformationShader.SetFloat("deformRadius", deformRadius);
        plasticDeformationShader.SetFloat("maxDeform", maxDeform);
        plasticDeformationShader.SetFloat("damageFalloff", damageFalloff);
        plasticDeformationShader.SetFloat("damageMultiplier", damageMultiplier);
        plasticDeformationShader.SetFloat("minDamage", minDamage);
        
        plasticDeformationShader.SetBuffer(0,"vertices", vertexBuffer);
        //plasticDeformationShader.SetFloat("resolution", vertices.Length);
        plasticDeformationShader.SetVector("impactPoint", impactPoint);
        
        plasticDeformationShader.Dispatch(0, vertices.Length/1024,1,1);
        
        vertexBuffer.GetData(vertices);
        
        //do the mesh update
        meshFilter.mesh.vertices = vertices;
        //meshFilter.mesh.RecalculateBounds();
        //meshFilter.mesh.RecalculateNormals();
        //meshFilter.mesh.RecalculateTangents();
        //meshCollider.sharedMesh = meshFilter.mesh;
        vertexBuffer.Dispose();
    }

    private void OnCPUCall()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertexPosition = vertices[i];
            float distanceFromCollision = Vector3.Distance(vertexPosition, impactPoint);
            float distanceFromOriginal = Vector3.Distance(defaultVertices[i], vertexPosition);

            if (distanceFromCollision < deformRadius && distanceFromOriginal < maxDeform)
            {
                float falloff = 1 - (distanceFromCollision / deformRadius) * damageFalloff;

                float xDeform = impactPoint.x * falloff;
                float yDeform = impactPoint.y * falloff;
                float zDeform = impactPoint.z * falloff;

                xDeform = Mathf.Clamp(xDeform, 0, maxDeform);
                yDeform = Mathf.Clamp(yDeform, 0, maxDeform);
                zDeform = Mathf.Clamp(zDeform, 0, maxDeform);

                Vector3 deform = new Vector3(xDeform, yDeform, zDeform);
                vertices[i] -= deform * damageMultiplier;
            }
        }
        //apply changes
        meshFilter.mesh.vertices = vertices;
        meshCollider.sharedMesh = meshFilter.mesh;
    }
    public void ToggleGPU()
    {
        useGPU = !useGPU;
    }
    private void OnGUI()
    {
        /*
        if (GUI.Button(new Rect(0, 0, 100, 50), "GPU Call"))
        {
            OnGPUCall();
        }
        */
    }

    private void OnDestroy()
    {
        
    }
}
