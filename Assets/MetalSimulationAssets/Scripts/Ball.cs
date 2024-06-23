using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private void OnEnable()
    {
        Destroy(gameObject,15);
    }

    private void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject);
    }
}
