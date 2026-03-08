using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    private struct RaycastHitCheck
    {
        public bool success;
        public RaycastHit hitInfo;
    }

    [SerializeField]
    private GameObject _seedPrefab;

    [SerializeField]
    private GameObject _bulletPrefab;

    [SerializeField]
    private DynamicReticle _dynamicReticle;

    [SerializeField]
    private Transform _seedSpawnPoint;

    [SerializeField]
    private Transform _defaultTarget;

    private bool _setUpComplete = false;

    [SerializeField]
    private LayerMask _raycastLayer;

    //private float _rayTraversalIncrement = 0.1f;

    RaycastHitCheck hitCheck = new RaycastHitCheck();

    private void Awake()
    {
        //this._seedSpawnPoint = Camera.main.transform;
    }

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);

        this._setUpComplete = true;
    }

    private void Update()
    {
        this.CastCameraRay();

        if (hitCheck.success == true)
        {
            this._dynamicReticle.UpdateReticlePosition(hitCheck.hitInfo.point, hitCheck.hitInfo.normal);
        }
        else
        {
            this._dynamicReticle.UpdateReticlePosition(Camera.main.transform.position, Camera.main.transform.forward);
        }
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

            this.ShootProjectile(this._seedPrefab);
        }

        if (PlayerControlsManager.instance.shootAltInitiated == true)
        {
            PlayerControlsManager.instance.shootAltInitiated = false;

            this.ShootProjectile(this._bulletPrefab);
        }
    }

    private void ShootProjectile(GameObject projectile)
    {
        GameObject projectileInstance = Instantiate(projectile, this._seedSpawnPoint.position, new Quaternion());
        ProjectileController  projectileController = projectileInstance.GetComponent<ProjectileController>();

        if (this.hitCheck.success == true)
        {
            projectileController.Launch(this.hitCheck.hitInfo.point);
        }
        else
        {
            projectileController.Launch(this._defaultTarget.position);
        }
    }

    private void ShootSeed()
    {
        GameObject seedInstance = Instantiate(this._seedPrefab, this._seedSpawnPoint.position, new Quaternion());
        SeedController seedController = seedInstance.GetComponent<SeedController>();

        if (this.hitCheck.success == true)
        {
            seedController.Launch(this.hitCheck.hitInfo.point);
        }
        else
        {
            seedController.Launch(this._defaultTarget.position);
        }       
    }

    private void ShootBullet()
    {
        GameObject bulletInstance = Instantiate(this._bulletPrefab, this._seedSpawnPoint.position, new Quaternion());
        BulletController bulletController = bulletInstance.GetComponent<BulletController>();

        if (this.hitCheck.success == true)
        {
            bulletController.Launch(this.hitCheck.hitInfo.point);
        }
        else
        {
            bulletController.Launch(this._defaultTarget.position);
        }
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

    private void CastCameraRay()
    {
        Ray cameraRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        this.hitCheck = new RaycastHitCheck();


        if (Physics.Raycast(cameraRay, out hitCheck.hitInfo, 100.0f, this._raycastLayer) == true)
        {
            this.hitCheck.success = true;
        }
        else
        {
            this.hitCheck.success = false;
        }
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
