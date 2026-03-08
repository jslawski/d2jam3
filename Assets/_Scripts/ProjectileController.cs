using System.Collections;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField]
    protected Rigidbody projectileRigidbody;
    [SerializeField]
    protected float launchSpeed;

    protected float expiryTime = 5.0f;

    public void Launch(Vector3 destinationPoint)
    {
        StartCoroutine(this.MoveProjectile(destinationPoint));
    
        StartCoroutine(this.ExpireAfterTime());
    }

    protected IEnumerator MoveProjectile(Vector3 destinationPoint)
    {
        Vector3 startingPoint = this.projectileRigidbody.position;

        int safetyIncrements = 100;

        while (Vector3.Distance(startingPoint, destinationPoint) <= 0.1f || safetyIncrements > 0)
        {
            this.projectileRigidbody.position = Vector3.Lerp(this.projectileRigidbody.position, destinationPoint, this.launchSpeed * Time.fixedDeltaTime);
            safetyIncrements--;

            yield return new WaitForFixedUpdate();
        }

        this.projectileRigidbody.position = destinationPoint;
    }

    protected IEnumerator ExpireAfterTime()
    {
        yield return new WaitForSeconds(this.expiryTime);

        Destroy(this.gameObject);
    }

    protected void OnDestroy()
    {
        StopAllCoroutines();
    }
}
