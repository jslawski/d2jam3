using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedController : ProjectileController
{
    [SerializeField]
    private GameObject _plantPrefab;

    private float _timeToSpawnPlant = 0.3f;

    private void OnCollisionEnter(Collision collision)
    {    
        if (collision.gameObject.tag == "Plantable")
        {
            ContactPoint collisionPoint = collision.contacts[0];
            this.projectileRigidbody.velocity = Vector3.zero;
            this.projectileRigidbody.gameObject.transform.position = collisionPoint.point;
            this.projectileRigidbody.isKinematic = true;

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
