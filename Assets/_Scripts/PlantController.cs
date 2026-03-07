using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PlantController : MonoBehaviour
{
    [SerializeField]
    private Transform _stemTransform;
    [SerializeField]
    private Transform _budTransform;

    private float _plantMaxLength = 5.0f;

    private float _timeToGrow = 0.5f;

    private float _timeToBud = 0.4f;

    private Vector3 _budFinalScale = new Vector3(2.5f, 2.0f, 2.5f);

    public void GrowPlant(Vector3 growthNormal)
    {
        Vector3 stemFinalPosition = this._stemTransform.position + (growthNormal * (0.5f * this._plantMaxLength));
        Vector3 budFinalPosition = this._stemTransform.position + (growthNormal * this._plantMaxLength);

        Sequence growSequence = DOTween.Sequence();

        Tweener stemGrowTween = this._stemTransform.DOScaleZ(this._plantMaxLength, this._timeToGrow).SetEase(Ease.InOutBack);
        Tweener stemMoveTween = this._stemTransform.DOMove(stemFinalPosition, this._timeToGrow).SetEase(Ease.InOutBack);

        Tweener budMoveTween = this._budTransform.DOMove(budFinalPosition, this._timeToGrow).SetEase(Ease.InOutBack);
        Tweener budGrowTween = this._budTransform.DOScale(this._budFinalScale, this._timeToBud).SetEase(Ease.InOutBack);

        growSequence.Append(stemGrowTween)
        .Join(stemMoveTween)
        .Join(budMoveTween)
        .Append(budGrowTween);
    }
}
