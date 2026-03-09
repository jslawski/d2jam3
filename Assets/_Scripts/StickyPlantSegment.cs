using DG.Tweening;
using UnityEngine;

public class StickyPlantSegment : PlantController
{
    private GameObject _nextSegmentObject;

    private float _segmentLength = 1.0f;

    [SerializeField]
    private Transform _previousSegmentParent;

    private Vector3 _growthNormal;

    private Transform _newSegmentParent;
    private Vector3 _newSegmentStartPosition = Vector3.zero;

    private StickyPlant _stickyPlantController;

    public void GrowSegment(StickyPlant stickyPlantController, Vector3 growthNormal, Transform parentTransform)
    {
        this._timeToGrow = 0.1f;

        this._nextSegmentObject = Resources.Load<GameObject>("StickySegment");
        
        this._stickyPlantController = stickyPlantController;
        this._growthNormal = growthNormal;
        this._previousSegmentParent = parentTransform;

        //Debug.LogError("GrowthNormal: " + growthNormal + " Magnitude: " + growthNormal.magnitude);

        Vector3 stemFinalPosition = this._stemTransform.position + (growthNormal * this._segmentLength);

        this._newSegmentParent = this._stemTransform;
        this._newSegmentStartPosition = stemFinalPosition + (growthNormal * this._segmentLength);

        DOTween.Sequence().Kill();

        Sequence growSequence = DOTween.Sequence();

        Tweener stemGrowTween = this._stemTransform.DOScaleY(this._segmentLength, this._timeToGrow).SetEase(Ease.Linear);
        Tweener stemMoveTween = this._stemTransform.DOMove(stemFinalPosition, this._timeToGrow).SetEase(Ease.Linear);

        TweenCallback nextSegmentCallback = this.GrowNextVineSegment;

        growSequence.Append(stemMoveTween)
        .Join(stemGrowTween)
        .AppendCallback(nextSegmentCallback);        
    }

    private void GrowNextVineSegment()
    {
        this._stickyPlantController.IncrementSegmentCount();        

        if (this._stickyPlantController.ShouldStopGrowing(this._segmentLength) == true)
        {
            this._stickyPlantController.StopGrowth();
            return;
        }
        else 
        {
            GameObject nextSegment = Instantiate(this._nextSegmentObject, this._newSegmentStartPosition, new Quaternion());            
            nextSegment.transform.up = this._growthNormal;
            nextSegment.name = "Segment" + this._stickyPlantController._currentSegments.ToString();

            StickyPlantSegment stickyPlantSegment = nextSegment.GetComponent<StickyPlantSegment>();

            this._stickyPlantController.AddSegmentToList(stickyPlantSegment);

            stickyPlantSegment.GrowSegment(this._stickyPlantController, this._growthNormal, this._newSegmentParent);
        }    
    }

    public void AttachToPreviousSegment()
    {    
        this.gameObject.transform.parent = this._previousSegmentParent;
    }

    public override void DestroyPlant(bool softDestroy = false)
    {
        this._stickyPlantController.DestroyPlant();
    }
}
