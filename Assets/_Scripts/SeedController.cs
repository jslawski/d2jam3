using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedController : ProjectileController
{
    [SerializeField]
    private GameObject _plantPrefab;

    private float _timeToSpawnPlant = 0.3f;

    [SerializeField]
    private GameObject _flowerBurstParticlePrefab;

    private void OnCollisionEnter(Collision collision)
    {    
        if (collision.gameObject.tag == "Plantable")
        {
            ContactPoint collisionPoint = collision.contacts[0];
            this.projectileRigidbody.velocity = Vector3.zero;
            this.projectileRigidbody.gameObject.transform.position = collisionPoint.point;
            this.projectileRigidbody.isKinematic = true;

            this.gameObject.transform.parent = this.GetTopLevelParent(collision.gameObject.transform);

            Instantiate(this._flowerBurstParticlePrefab, this.gameObject.transform.position, new Quaternion());            

            StartCoroutine(this.SpawnPlant(collisionPoint, this.GetTopLevelParent(collision.gameObject.transform)));

            this.collisionFound = true;
        }
        else if (collision.gameObject.tag == "Plant" || collision.gameObject.tag == "DirtPlant")
        {            
            Destroy(this.gameObject);

            this.collisionFound = true;
        }
    }

    private IEnumerator SpawnPlant(ContactPoint collisionPoint, Transform parentTransform)
    {
        yield return new WaitForSeconds(this._timeToSpawnPlant);

        GameObject plantInstance = Instantiate(this._plantPrefab, this.gameObject.transform.position, new Quaternion());
        plantInstance.transform.up = collisionPoint.normal;

        PlantController plantController = plantInstance.GetComponent<PlantController>();
        plantController.GrowPlant(collisionPoint.normal, parentTransform);

        Destroy(this.gameObject);
    }

    private Transform GetTopLevelParent(Transform initialTransform)
    {
        Transform currentParent = initialTransform;

        while ((currentParent.parent != null))
        {
            currentParent = currentParent.parent;

            if (currentParent.name.Contains("Plant"))
            {
                break;
            }
        }

        Debug.LogError("Selected Parent: " + currentParent.gameObject.name);

        return currentParent;
    }
}
