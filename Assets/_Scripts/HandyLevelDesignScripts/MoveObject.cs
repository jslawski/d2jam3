using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    private Transform _objectTransform;

    [Header("Which positions should the object move between?")]
    [SerializeField]
    private Vector3 _position1 = Vector3.zero;
    [SerializeField]
    private Vector3 _position2 = Vector3.zero;

    [Header("How long should it take to get from Position 1 to Position 2?")]
    [SerializeField]
    private float _moveTime = 2.0f;

    [Header("How long should the object stay at a destination before moving again?")]
    [SerializeField]
    private float _waitTime = 1.0f;

    private bool _moveToPosition2 = true;

    private void Awake()
    {
        this._objectTransform = this.gameObject.GetComponent<Transform>();
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        this._objectTransform.position = this._position1;
        StartCoroutine(this.MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(this._waitTime);

            if (this._moveToPosition2 == true)
            {
                this._objectTransform.DOMove(this._position2, this._moveTime);
            }
            else
            {
                this._objectTransform.DOMove(this._position1, this._moveTime);
            }

            this._moveToPosition2 = !this._moveToPosition2;

            yield return new WaitForSeconds(this._moveTime);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
