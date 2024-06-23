using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShootBalls : MonoBehaviour
{
    [SerializeField] private GameObject prefabToInstantiate;
    [SerializeField] private float speed = 7.0f;
    [SerializeField] private Camera cam;
    [SerializeField] private bool canShoot = true;
    

    private void Start()
    {
        cam = Camera.main;
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        StartCoroutine(CheckClick());
    }
    
    private IEnumerator CheckClick()
    {
        yield return new WaitForEndOfFrame();

        if (EventSystem.current.IsPointerOverGameObject())
        {
            //Debug.Log("pointer over ui do nothing");
        }
        else
        {
            if (canShoot)
            {
                Ray mouseRay = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
                GameObject newGameObject = Instantiate(prefabToInstantiate, mouseRay.origin, Quaternion.identity);
                Rigidbody rb = newGameObject.GetComponent<Rigidbody>();
 
                if (rb != null)
                {
                    rb.velocity = mouseRay.direction * speed;
                }
            }
        }
    }

    public void ToggleShoot()
    {
        canShoot = !canShoot;
    }
    
}
