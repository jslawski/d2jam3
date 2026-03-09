using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtFlower : PlantController
{
    [SerializeField]
    private Transform _flowerBudTransform;

    private float _budUpYThreshold = -0.75f;

    public override void GrowPlant(Vector3 growthNormal, Transform parentTransform)
    {
        /*if (growthNormal.y > this._budUpYThreshold)
        {
            this._flowerBudTransform.up = Vector3.up;
        }
        */

        this.gameObject.transform.parent = parentTransform;
    }
}
