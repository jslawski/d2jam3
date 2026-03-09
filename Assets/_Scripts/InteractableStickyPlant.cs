using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableStickyPlant : PlantController
{    
    private int _maxLength = 20;
    

    [SerializeField]
    private LayerMask _raycastLayer;

    private bool _killGrowth = false;

    public override void GrowPlant(Vector3 growthNormal, Transform parentTransform)
    {        
        this._timeToGrow = 4.0f;
        this._parentTransform = parentTransform;               

        Vector3 stemFinalPosition = this._stemTransform.position + (growthNormal * (0.5f * this._maxLength));

        DOTween.Sequence().Kill();

        Sequence growSequence = DOTween.Sequence();

        Tweener stemGrowTween = this._stemTransform.DOScaleZ(this._maxLength, this._timeToGrow).SetEase(Ease.Linear);
        Tweener stemMoveTween = this._stemTransform.DOMove(stemFinalPosition, this._timeToGrow).SetEase(Ease.Linear);

        growSequence.Append(stemGrowTween)
        .Join(stemMoveTween);      
    }
}
