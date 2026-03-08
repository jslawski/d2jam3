using DG.Tweening;
using UnityEngine;

public class StickyPlantSegment : MonoBehaviour
{
    private GameObject _nextSegmentObject;
    [SerializeField]
    private Transform _stemTransform;

    private float _timeToGrow = 0.1f;
    private float _segmentLength = 3.0f;

    [SerializeField]
    private Transform _previousSegmentParent;

    private Vector3 _growthNormal;

    private Transform _newSegmentParent;
    private Vector3 _newSegmentStartPosition = Vector3.zero;

    private StickyPlant _stickyPlantController;

    public void GrowSegment(StickyPlant stickyPlantController, Vector3 growthNormal, Transform parentTransform)
    {
        this._nextSegmentObject = Resources.Load<GameObject>("StickyPlantSegment");

        this._stemTransform.localScale = new Vector3(1.0f, 1.0f, 0.1f);
        
        this._stickyPlantController = stickyPlantController;
        this._growthNormal = growthNormal;
        this._previousSegmentParent = parentTransform;

        Vector3 stemFinalPosition = this._stemTransform.position + (growthNormal * (0.5f * this._segmentLength));

        this._newSegmentParent = this._stemTransform;
        this._newSegmentStartPosition = stemFinalPosition + (growthNormal * (0.5f * this._segmentLength));

        DOTween.Sequence().Kill();

        Sequence growSequence = DOTween.Sequence();

        Tweener stemGrowTween = this._stemTransform.DOScaleZ(this._segmentLength, this._timeToGrow).SetEase(Ease.Linear);
        Tweener stemMoveTween = this._stemTransform.DOMove(stemFinalPosition, this._timeToGrow).SetEase(Ease.Linear);

        TweenCallback nextSegmentCallback = this.GrowNextVineSegment;

        growSequence.Append(stemGrowTween)
        .Join(stemMoveTween)
        .AppendCallback(nextSegmentCallback);        
    }

    private void GrowNextVineSegment()
    {
        this._stickyPlantController.IncrementSegmentCount();        

        if (this._stickyPlantController.ShouldStopGrowing() == true)
        {
            this._stickyPlantController.StopGrowth();
            return;
        }
        else 
        {
            GameObject nextSegment = Instantiate(this._nextSegmentObject, this._newSegmentStartPosition, new Quaternion());            
            nextSegment.transform.forward = this._growthNormal;
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
}
