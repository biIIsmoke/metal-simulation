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
    [SerializeField] private GameObject prefabSphere;
    [SerializeField] private GameObject sphere;
    [SerializeField] private Grabbable grabbable;
    
    [SerializeField] private InputAction mouseClick;
    [SerializeField] private InputAction mouseWheel;
    [SerializeField] private bool canGrab = false;
    [SerializeField] private bool grabbing;
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
        mouseClick.started += MousePressed;
        mouseClick.canceled += MouseReleased;
        mouseWheel.performed += WheelTurned;
        grabbing = false;
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

        if (grabbing)
        {
            //call compute shader using sphere data
            //Debug.Log($"left click hold{sphere.transform.localScale / 2}");
            grabbable.CallShader(sphere.transform.position, sphere.transform.localScale.x / 2); //updates current position
        }
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
        //mouse released call compute shader here
        StartCoroutine(CheckClick());
    }
    
    private void MouseReleased(InputAction.CallbackContext context)
    {
        //mouse released
        //Debug.Log("left click is released");
        grabbing = false;
        grabbable.SetGrabPosition(new Vector3(1500,1500,1500)); //make it so far :D
    }
    
    private IEnumerator CheckClick()
    {
        // Wait until the end of the frame
        yield return new WaitForEndOfFrame();

        if (EventSystem.current.IsPointerOverGameObject())
        {
            //Debug.Log("pointer over ui do nothing");
        }
        else
        {
            if (canGrab)
            {
                grabbable.SetGrabPosition(sphere.transform.position);
                //Debug.Log("left click is pressed");
                grabbing = true;
            }
        }
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
