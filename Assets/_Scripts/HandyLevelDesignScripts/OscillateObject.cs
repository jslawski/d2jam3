using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OsscilateObject : MonoBehaviour
{
    private Transform _objectTransform;

    [Header("Which angles should the object change between?")]
    [SerializeField]
    private float _rotation1 = 0.5f;
    [SerializeField]
    private float _rotation2 = 1.5f;

    [Header("How long should it take to get from Rotation 1 to Rotation 2?")]
    [SerializeField]
    private float _moveTime = 0.5f;

    [Header("How long should the object stay at a target angle before rotating again?")]
    [SerializeField]
    private float _waitTime = 2.0f;

    private bool _changeToRotation2 = true;

    private Vector3 _rotationVector1;
    private Vector3 _rotationVector2;

    private void Awake()
    {
        this._objectTransform = this.gameObject.GetComponent<Transform>();

        this._rotationVector1 = new Vector3(0.0f, 0.0f, this._rotation1);
        this._rotationVector2 = new Vector3(0.0f, 0.0f, this._rotation2);
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        this._objectTransform.localRotation = Quaternion.Euler(this._rotationVector1);
        StartCoroutine(this.MoveCoroutine());

    }

    private IEnumerator MoveCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(this._waitTime);

            if (this._changeToRotation2 == true)
            {
                this._objectTransform.DORotate(this._rotationVector2, this._moveTime);
            }
            else
            {
                this._objectTransform.DORotate(this._rotationVector1, this._moveTime);
            }

            this._changeToRotation2 = !this._changeToRotation2;

            yield return new WaitForSeconds(this._moveTime);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
