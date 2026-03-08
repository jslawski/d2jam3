using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicReticle : MonoBehaviour
{
    [SerializeField]
    private Transform _reticleTransform;
    [SerializeField]
    private Transform _spriteTransform;

    private float _spinSpeed = 90.0f;

    private void Update()
    {
        this._spriteTransform.Rotate(this._spriteTransform.forward, this._spinSpeed * Time.deltaTime, Space.World);
    }

    public void UpdateReticlePosition(Vector3 position, Vector3 normal)
    {
        this._reticleTransform.position = position;
        this._reticleTransform.forward = normal;        
    }
}
