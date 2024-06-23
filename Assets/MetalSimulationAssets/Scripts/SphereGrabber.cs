using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SphereGrabber : MonoBehaviour
{
    [SerializeField] private InputAction mouseClick;
    [SerializeField] private InputAction mouseWheel;
    [SerializeField] private bool canGrab = false;
    [SerializeField] private GameObject sphere;
    [SerializeField] private Vector3 worldPosition;
    [SerializeField] private float mouseDragSpeed = .05f;
    [SerializeField] private float zDisplacement = 0f;

    private Vector3 velocity = Vector3.zero;
    
    
    //just follows the mouse position and projects a sphere at that point. if player clicks, it sends that sphere data to compute shader
    //and uses it to transform vertices, create new ones if rupture happens
    
    [SerializeField] private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        mouseClick.Enable();
        mouseWheel.Enable();
        mouseClick.performed += MousePressed;
        mouseWheel.performed += WheelTurned;
    }
    
    void Update()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = Camera.main.nearClipPlane + 5.0f + zDisplacement;
        worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        sphere.transform.position = Vector3.SmoothDamp(sphere.transform.position, worldPosition, ref velocity, mouseDragSpeed);
    }

    private void OnDisable()
    {
        mouseClick.performed -= MousePressed;
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
        //mouse pressed call compute shader here
        
    }

    public void ToggleGrab()
    {
        canGrab = !canGrab;
        if (canGrab)
        {
            //create a sphere in worldPosition
            sphere = Instantiate(sphere, worldPosition, quaternion.identity);
        }
    }
    
}
