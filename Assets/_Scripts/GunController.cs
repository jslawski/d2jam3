using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField]
    private GameObject _seedPrefab;

    [SerializeField]
    private GameObject _bulletPrefab;

    [SerializeField]
    private Transform _seedSpawnPoint;

    [SerializeField]
    private Transform _defaultTarget;

    private bool _setUpComplete = false;

    //private float _rayTraversalIncrement = 0.1f;
    
    // Start is called before the first frame update
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);

        this._setUpComplete = true;
    }
    
    void FixedUpdate()
    {
        if (this._setUpComplete == false)
        {
            PlayerControlsManager.instance.shootInitiated = false;   
            return;
        }
    
        if (PlayerControlsManager.instance.shootInitiated == true)
        { 
            PlayerControlsManager.instance.shootInitiated = false;

            this.ShootSeed();
        }

        if (PlayerControlsManager.instance.shootAltInitiated == true)
        {
            PlayerControlsManager.instance.shootAltInitiated = false;

            this.ShootBullet();
        }
    }

    private void ShootSeed()
    {
        GameObject seedInstance = Instantiate(this._seedPrefab, this._seedSpawnPoint.position, new Quaternion());
        SeedController seedController = seedInstance.GetComponent<SeedController>();

        seedController.LaunchSeed(this.GetTrajectory());
    }

    private void ShootBullet()
    {
        GameObject bulletInstance = Instantiate(this._bulletPrefab, this._seedSpawnPoint.position, new Quaternion());
        BulletController bulletController = bulletInstance.GetComponent<BulletController>();

        bulletController.LaunchBullet(this.GetTrajectory());
    }

    private Vector3 GetTrajectory()
    {
        Ray cameraRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hitInfo = new RaycastHit();

        Vector3 targetPosition = this._defaultTarget.position;
        
        if (Physics.Raycast(cameraRay, out hitInfo, 100.0f))
        {
            targetPosition = hitInfo.point;
        }
    
        return (targetPosition - this._seedSpawnPoint.position).normalized;
    }

    /*
    private Vector3 GetTrajectory()
    { 
        Ray cameraRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        Ray gunRay = new Ray(this._seedSpawnPoint.position, this._seedSpawnPoint.forward);

        float currentIncrement = 0;

        float currentDistance = Vector3.Distance(cameraRay.GetPoint(currentIncrement), gunRay.GetPoint(currentIncrement));
        float minDistance = float.NegativeInfinity;

        float safetyIncrement = 100f;

        while (currentDistance > minDistance || currentIncrement < safetyIncrement)
        {
            minDistance = currentDistance;

            currentIncrement += this._rayTraversalIncrement;
            currentDistance = Vector3.Distance(cameraRay.GetPoint(currentIncrement), gunRay.GetPoint(currentIncrement));

            
        }

        Debug.LogError("Min Distance: " + minDistance + " Current Increment: " + currentIncrement);

        return gunRay.GetPoint(minDistance);
    }
    */
}
