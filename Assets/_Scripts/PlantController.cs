using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PlantController : MonoBehaviour
{
    [SerializeField]
    private Transform _plantTransform;

    [SerializeField]
    private float _plantMaxLength = 3.0f;

    private float _timeToGrow = 0.5f;

    public void GrowPlant(Vector3 growthNormal)
    {
        Vector3 finalPosition = this._plantTransform.position + (growthNormal * (0.5f * this._plantMaxLength));
            
        this._plantTransform.DOScaleZ(this._plantMaxLength, this._timeToGrow).SetEase(Ease.InOutBack);
        this._plantTransform.DOMove(finalPosition, this._timeToGrow).SetEase(Ease.InOutBack);
    }
}
