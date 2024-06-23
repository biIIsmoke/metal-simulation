using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SphereGrabber : MonoBehaviour
{
    [SerializeField] private bool canGrab = false;
    
    //just follows the mouse position and projects a sphere at that point. if player clicks, it sends that sphere data to compute shader
    //and uses it to transform vertices, create new ones if rupture happens
    
    

    // Update is called once per frame
    private void Update()
    {
        if (canGrab) //do stuff
        {
            
        }
    }

    public void ToggleGrab()
    {
        canGrab = !canGrab;
    }
    
}
