using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _bulletRigidbody;

    [SerializeField]
    private float _launchSpeed;

    private float _expiryTime = 5.0f;

    public void LaunchBullet(Vector3 trajectory)
    {
        this._bulletRigidbody.velocity = trajectory * this._launchSpeed;

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
            Destroy(this.gameObject);
        }

        if (collision.gameObject.tag == "Plant")
        {
            Destroy(collision.gameObject.transform.parent.gameObject);
            Destroy(this.gameObject);
        }
    }
}
