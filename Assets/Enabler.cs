using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enabler : MonoBehaviour
{
    private void OnEnable()
    {
        Debug.LogError(transform.root.name + " Enabled");
    }
    private void OnDisable()
    {
        Debug.LogError(transform.root.name + " Disabled");
    }
}
