using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedController : MonoBehaviour
{
    [SerializeField]    
    private Rigidbody _seedRigidbody;

    [SerializeField]
    private float _launchSpeed;

    private float _expiryTime = 5.0f;

    private ContactPoint _collisionPoint;

    public void LaunchSeed(Vector3 trajectory)
    {
        this._seedRigidbody.velocity = trajectory * this._launchSpeed;

        StartCoroutine(this.ExpireAfterTime());
    }

    private IEnumerator ExpireAfterTime()
    {
        yield return new WaitForSeconds(this._expiryTime);

        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Plantable")
        {
            this._collisionPoint = collision.contacts[0];
            this._seedRigidbody.velocity = Vector3.zero;
            this._seedRigidbody.gameObject.transform.position = this._collisionPoint.point;
            this._seedRigidbody.isKinematic = true;
        }
    }
}
