using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    private Transform _objectTransform;

    [SerializeField]
    private bool xAxis = false;
    [SerializeField]
    private bool yAxis = false;
    [SerializeField]
    private bool zAxis = false;

    [SerializeField]
    [Range(-2.0f, 2.0f)]
    private float rotationSpeed = 1.0f;

    private void Awake()
    {
        this._objectTransform = this.gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 rotationAxis = Vector3.zero;

        if (this.xAxis == true)
        {
            rotationAxis += Vector3.right;
        }
        
        if (this.yAxis == true) 
        {
            rotationAxis += Vector3.up;
        }
        
        if (this.zAxis == true)         
        {
            rotationAxis += Vector3.forward;
        }

        this._objectTransform.rotation *= Quaternion.AngleAxis(rotationSpeed, rotationAxis);
    }
}
