using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedController : MonoBehaviour
{
    [SerializeField]
    private GameObject _plantPrefab;
    
    [SerializeField]    
    private Rigidbody _seedRigidbody;

    [SerializeField]
    private float _launchSpeed;

    private float _expiryTime = 5.0f;

    private float _timeToSpawnPlant = 0.3f;

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
            ContactPoint collisionPoint = collision.contacts[0];
            this._seedRigidbody.velocity = Vector3.zero;
            this._seedRigidbody.gameObject.transform.position = collisionPoint.point;
            this._seedRigidbody.isKinematic = true;

            StartCoroutine(this.SpawnPlant(collisionPoint));
        }
        else if (collision.gameObject.tag == "Plant")
        {
            Destroy(this.gameObject);
        }
    }

    private IEnumerator SpawnPlant(ContactPoint collisionPoint)
    {
        yield return new WaitForSeconds(this._timeToSpawnPlant);

        GameObject plantInstance = Instantiate(this._plantPrefab, collisionPoint.point, new Quaternion());
        plantInstance.transform.forward = collisionPoint.normal;

        PlantController plantController = plantInstance.GetComponent<PlantController>();
        plantController.GrowPlant(collisionPoint.normal);

        Destroy(this.gameObject);
    }
}
