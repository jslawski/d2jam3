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

        Tweener stemGrowTween = this._stemTransform.DOScaleZ(this._plantMaxLength, this._timeToGrow).SetEase(Ease.OutBack);
        Tweener stemMoveTween = this._stemTransform.DOMove(stemFinalPosition, this._timeToGrow).SetEase(Ease.OutBack);

        Tweener budMoveTween = this._budTransform.DOMove(budFinalPosition, this._timeToGrow).SetEase(Ease.OutBack);
        Tweener budGrowTween = this._budTransform.DOScale(this._budFinalScale, this._timeToBud).SetEase(Ease.OutBack);

        growSequence.Append(stemGrowTween)
        .Join(stemMoveTween)
        .Join(budMoveTween)
        .Join(budGrowTween);
    }

    public void DestroyPlant()
    {
        GameObject budObject = this._budTransform.gameObject;
        GameObject stemObject = this._stemTransform.gameObject;

        budObject.transform.parent = null;
        stemObject.transform.parent = null;

        budObject.layer = 0;
        stemObject.layer = 0;

        budObject.tag = "Untagged";
        stemObject.tag = "Untagged";

        Rigidbody budRb = budObject.AddComponent<Rigidbody>();
        Rigidbody stemRb = stemObject.AddComponent<Rigidbody>();

        budRb.useGravity = true;
        stemRb.useGravity = true;

        float randomMagnitude = Random.Range(10.0f, 50.0f);
        Vector3 randomDirection = Random.onUnitSphere;

        budRb.AddForce(randomDirection * randomMagnitude, ForceMode.Impulse);
        
        randomMagnitude = Random.Range(10.0f, 50.0f);
        randomDirection = Random.onUnitSphere;

        stemRb.AddForce(randomDirection * randomMagnitude, ForceMode.Impulse);



        StartCoroutine(this.DestroyAfterDelay(budObject, stemObject));
    }

    private IEnumerator DestroyAfterDelay(GameObject budObject, GameObject stemObject)
    {
        yield return new WaitForSeconds(3.0f);

        Destroy(budObject);
        Destroy(stemObject);
        Destroy(this.gameObject);
    }
}
