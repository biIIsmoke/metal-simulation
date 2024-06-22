using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody rb;
    private void OnEnable()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        Destroy(gameObject,15);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Deformable"))
        {
            ContactPoint contactPoint = other.GetContact(0);
            Debug.Log(contactPoint.point);
        
            PlasticDeformationController controller = other.gameObject.GetComponent<PlasticDeformationController>();
        
            controller.impactPoint = contactPoint.point;
            controller.impactVector = rb.velocity;
        
            Debug.Log(controller.impactVector);
        
            controller.OnGPUCall();
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
