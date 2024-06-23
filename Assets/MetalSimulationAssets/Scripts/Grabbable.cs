using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct GrabSphere
{
    private Vector3 position;
    private float radius;
}

public class Grabbable : MonoBehaviour
{
    [SerializeField] private ComputeShader grabShader;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
