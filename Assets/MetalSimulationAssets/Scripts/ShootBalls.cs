using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootBalls : MonoBehaviour
{
    [SerializeField] private GameObject prefabToInstantiate;
    [SerializeField] private float speed = 7.0f;
    [SerializeField] private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    public void OnClick(InputAction.CallbackContext context)
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
