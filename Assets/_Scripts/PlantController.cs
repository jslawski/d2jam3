using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PlantController : MonoBehaviour
{
    [SerializeField]
    protected Transform _stemTransform;
    [SerializeField]
    protected Transform _budTransform;

    protected float _plantMaxLength = 5.0f;

    protected float _timeToGrow = 0.5f;

    protected float _timeToBud = 0.4f;

    [SerializeField]
    protected Vector3 _budFinalScale = new Vector3(2.5f, 2.0f, 2.5f);

    protected Transform _parentTransform;

    public bool isSticky = false;

    public GameObject stuckObject;

    public virtual void GrowPlant(Vector3 growthNormal, Transform parentTransform)
    {
        this._parentTransform = parentTransform;
        this.AttachToParent();
        
        Vector3 stemFinalPosition = this._stemTransform.position + (growthNormal * (0.5f * this._plantMaxLength));
        Vector3 budFinalPosition = this._stemTransform.position + (growthNormal * this._plantMaxLength);        

        Sequence growSequence = DOTween.Sequence();

        Tweener stemGrowTween = this._stemTransform.DOScaleY(this._plantMaxLength, this._timeToGrow).SetEase(Ease.OutBack);
        Tweener stemMoveTween = this._stemTransform.DOMove(stemFinalPosition, this._timeToGrow).SetEase(Ease.OutBack);        

        Tweener budMoveTween = this._budTransform.DOMove(budFinalPosition, this._timeToGrow).SetEase(Ease.OutBack);
        Tweener budGrowTween = this._budTransform.DOScale(this._budFinalScale, this._timeToBud).SetEase(Ease.OutBack);

        TweenCallback parentCallback = this.AttachToParent;

        growSequence.Append(stemGrowTween)
        .Join(stemMoveTween)
        .Join(budMoveTween)
        .Join(budGrowTween)
        .AppendCallback(parentCallback);
        
         


    }

    protected void AttachToParent()
    {
        //if (this._parentTransform.gameObject.layer == LayerMask.NameToLayer("Plant"))
        //{
            this.gameObject.transform.parent = this._parentTransform;
        //}            
    }

    public virtual void DestroyPlant(bool softDestroy = false)
    {    
        Transform[] plantParts = this.GetComponentsInChildren<Transform>();

        for (int i = 0; i < plantParts.Length; i++)
        {
            Animator potentialAnimator = plantParts[i].gameObject.GetComponent<Animator>();


            if (potentialAnimator != null)
            {
                potentialAnimator.enabled = false;
            }
        
            plantParts[i].parent = null;
            plantParts[i].gameObject.layer = 0;
            plantParts[i].gameObject.tag = "Untagged";
            plantParts[i].gameObject.layer = LayerMask.NameToLayer("Destroyed");
            Rigidbody plantRb = plantParts[i].gameObject.AddComponent<Rigidbody>();
            plantRb.useGravity = true;

            float randomMagnitude;
            Vector3 randomDirection;

            if (softDestroy == true)
            {
                randomMagnitude = Random.Range(5.0f, 10.0f);

                float randomX = Random.Range(-0.3f, 0.3f);
                float randomZ = Random.Range(-0.3f, 0.3f);

                randomDirection = new Vector3(randomX, -1, randomZ).normalized;
            }
            else
            {
                randomMagnitude = Random.Range(10.0f, 15.0f);
                randomDirection = Random.onUnitSphere;
            }

            plantRb.AddForce(randomDirection * randomMagnitude, ForceMode.Impulse);
            plantRb.AddTorque(randomDirection * randomMagnitude, ForceMode.Impulse);

            StartCoroutine(this.DestroyAfterDelay(plantParts[i].gameObject));
        }
    }

    protected IEnumerator DestroyAfterDelay(GameObject flowerObject)
    {
        yield return new WaitForSeconds(3.0f);

        Destroy(flowerObject);
        Destroy(this.gameObject);
    }
}
