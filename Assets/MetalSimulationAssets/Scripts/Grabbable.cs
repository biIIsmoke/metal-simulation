using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grabbable : MonoBehaviour
{
    [SerializeField] private ComputeShader grabShader;
    private Vector3[] vertices; 
    private Vector3[] defaultVertices;
    [SerializeField] private Vector3 grabPosition;
    [SerializeField] private Vector3 worldPosition;
    [SerializeField] private float grabRadius;
    
    [Range(0,1)]
    [SerializeField] private float maxDeform = 0.2f;
    [Range(0,1)]
    [SerializeField] private float maxElasticity = 0.1f;
    
    int kernel;
    
    private MeshFilter meshFilter;
    private ComputeBuffer vertexBuffer;
    private ComputeBuffer defaultVertexBuffer;
    
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject prefabSphere;
    [SerializeField] private GameObject sphere;
    
    [SerializeField] private InputAction mouseClick;
    [SerializeField] private InputAction mouseWheel;
    [SerializeField] private bool canGrab = false;
    [SerializeField] private float mouseDragSpeed = .05f;
    [SerializeField] private float zDisplacement = 0f;

    private Vector3 velocity = Vector3.zero;
    private void Awake()
    {
        mainCamera = Camera.main;
    }
    
    private void OnEnable()
    {
        mouseClick.Enable();
        mouseWheel.Enable();
        mouseClick.started += MousePressed;
        mouseClick.canceled += MouseReleased;
        mouseWheel.performed += WheelTurned;
    }
    
    private void OnDisable()
    {
        mouseClick.started -= MousePressed;
        mouseClick.canceled -= MouseReleased;
        mouseWheel.performed -= WheelTurned;
        mouseClick.Disable();
        mouseWheel.Disable();
    }
    
    private void WheelTurned(InputAction.CallbackContext context)
    {
        Vector2 vec = Mouse.current.scroll.ReadValue();
        zDisplacement += vec.y/360;
    }
    
    private void MousePressed(InputAction.CallbackContext context)
    {
        if (canGrab)
        {
            grabPosition = sphere.transform.position;
            grabRadius = 0.5f;
        }
    }
    
    private void MouseReleased(InputAction.CallbackContext context)
    {
        //mouse released
        //Debug.Log("left click is released");
        CallShader();
    }
    // Start is called before the first frame update
    void Start()
    {
        kernel = grabShader.FindKernel("CSSecond");
        meshFilter = GetComponent<MeshFilter>();
        vertices = meshFilter.mesh.vertices;
        defaultVertices = meshFilter.mesh.vertices;
        
        //sends default vertex data to be used in calculations without change
        int vector3Size = sizeof(float) * 3;
        int totalSize = vector3Size;
        defaultVertexBuffer = new ComputeBuffer(defaultVertices.Length, totalSize);
        defaultVertexBuffer.SetData(defaultVertices);
        grabShader.SetBuffer(kernel,"defaultVertices", defaultVertexBuffer);
        defaultVertexBuffer.Dispose();
    }

    void Update()
    {
        if (canGrab)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            mousePos.z = Camera.main.nearClipPlane + 5.0f + zDisplacement;
            worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
            sphere.transform.position = Vector3.SmoothDamp(sphere.transform.position, worldPosition, ref velocity, mouseDragSpeed);
        }
    }
    
    public void CallShader()
    {
        Debug.Log("calling shader each frame");
        int vector3Size = sizeof(float) * 3;
        int totalSize = vector3Size;
        
        
        //set constraints, convert to local coordinates
        grabShader.SetVector("grabPosition", transform.InverseTransformPoint(grabPosition));
        grabShader.SetVector("currentPosition", transform.InverseTransformPoint(worldPosition));
        grabShader.SetFloat("grabRadius", grabRadius);
        grabShader.SetFloat("maxElasticity", maxElasticity);
        grabShader.SetFloat("maxDeform", maxDeform);
        
        vertexBuffer = new ComputeBuffer(vertices.Length, totalSize);
        vertexBuffer.SetData(vertices);
        
        grabShader.SetBuffer(kernel,"vertices", vertexBuffer);
        
        grabShader.Dispatch(kernel, vertices.Length/512,1,1);
        
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
    
    public void ToggleGrab()
    {
        canGrab = !canGrab;
        if (canGrab)
        {
            //create a sphere in worldPosition
            if (sphere == null)
            {
                sphere = Instantiate(prefabSphere, worldPosition, quaternion.identity);
            }
            else
            {
                sphere.gameObject.SetActive(true);
            }
        }
        else
        {
            sphere.gameObject.SetActive(false);
        }
    }
}
