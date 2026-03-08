using System.Collections.Generic;
using UnityEngine;

public class StickyPlant : PlantController
{
    private GameObject _segmentPrefab;

    public int _currentSegments;
    private int _maxSegments = 100;

    [SerializeField]
    private List<StickyPlantSegment> _segments;
    [SerializeField]
    private LayerMask _raycastLayer;

    private bool _killGrowth = false;

    public override void GrowPlant(Vector3 growthNormal, Transform parentTransform)
    {
        this._segmentPrefab = Resources.Load<GameObject>("StickyPlantSegment");
    
        this._timeToGrow = 0.2f;
        this._parentTransform = parentTransform;

        this._segments = new List<StickyPlantSegment>();

        GameObject nextSegment = Instantiate(this._segmentPrefab, this.gameObject.transform.position, new Quaternion());
        nextSegment.transform.forward = growthNormal;
        nextSegment.name = "Segment" + this._currentSegments.ToString();

        StickyPlantSegment stickyPlantSegment = nextSegment.GetComponent<StickyPlantSegment>();

        this.AddSegmentToList(stickyPlantSegment);

        stickyPlantSegment.GrowSegment(this, growthNormal, this.gameObject.transform);
    }

    public override void DestroyPlant(bool softDestroy = false)
    {
        this.LinkUpCurrentSegments();
    
        Transform[] plantParts = this.GetComponentsInChildren<Transform>();

        for (int i = 0; i < plantParts.Length; i++)
        {
            plantParts[i].parent = null;
            plantParts[i].gameObject.layer = 0;
            plantParts[i].gameObject.tag = "Untagged";
            Rigidbody plantRb = plantParts[i].gameObject.AddComponent<Rigidbody>();
            plantRb.useGravity = true;

            float randomMagnitude;
            Vector3 randomDirection;

            if (softDestroy == true)
            {
                randomMagnitude = UnityEngine.Random.Range(10.0f, 20.0f);

                float randomX = UnityEngine.Random.Range(-0.3f, 0.3f);
                float randomZ = UnityEngine.Random.Range(-0.3f, 0.3f);

                randomDirection = new Vector3(randomX, -1, randomZ).normalized;
            }
            else
            {
                randomMagnitude = UnityEngine.Random.Range(10.0f, 50.0f);
                randomDirection = UnityEngine.Random.onUnitSphere;
            }

            plantRb.AddForce(randomDirection * randomMagnitude, ForceMode.Impulse);
            plantRb.AddTorque(randomDirection * randomMagnitude, ForceMode.Impulse);

            StartCoroutine(this.DestroyAfterDelay(plantParts[i].gameObject));
        }
    }

    public void IncrementSegmentCount()
    {
        this._currentSegments++;
    }

    public bool ShouldStopGrowing(float distanceAhead)
    {        

        return (this._currentSegments >= this._maxSegments) || (this._killGrowth == true) || this.IsObstacleAhead(distanceAhead);
    }

    public bool IsObstacleAhead(float distanceAhead)
    {
        Vector3 originPoint = this.transform.position;
        float nextDistance = (this._segments.Count + 1) * distanceAhead;
        Vector3 destinationPoint = originPoint + (this.gameObject.transform.forward * nextDistance);

        if (Physics.Raycast(originPoint, this.gameObject.transform.forward, nextDistance, this._raycastLayer) == true)
        {
            return true;
        }

        return false;
    }

    public void StopGrowth()
    {
        this.LinkUpCurrentSegments();
        this.AttachToParent();
        this._killGrowth = true;
    }

    private void LinkUpCurrentSegments()
    {    
        for (int i = this._segments.Count - 1; i >= 0; i--) 
        {
            this._segments[i].AttachToPreviousSegment();
        }        
    }

    public void AddSegmentToList(StickyPlantSegment newSegment)
    {
        this._segments.Add(newSegment);
    }
}
