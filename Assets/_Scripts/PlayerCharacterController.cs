using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacterController : MonoBehaviour
{
    private Transform _cameraTransform;
    private Rigidbody _playerRigidbody;

    [SerializeField]
    private Collider _playerFeetCollider;

    private Vector3 _moveDirection = Vector3.zero;
    private float _moveAcceleration = 5.0f;
    private float _maxMoveVelocity = 15.0f;
    private Coroutine _moveLerpCoroutine = null;
    
    private Quaternion _targetRotation = Quaternion.identity;
    private float _cameraRotateSpeed = 15.0f;
    private Coroutine _cameraSlerpCoroutine = null;


    private float _isGroundedDistance = 0.1f;
    private float _maxVerticalAngle = 90.0f;
    private float _jumpForce = 20.0f;
    private float _unstickJumpForce = 40.0f;
    private bool _jumpBuffered = false;

    private bool _isGroundedThisFrame = false;

    private Vector3 _stuckJumpNormal = Vector3.zero;
    private Vector3 _stuckMoveDirection = Vector3.zero;
    private Vector3 _localStuckContactPointOffset = Vector3.zero;

    public GunController gunController;

    public GameObject debugObject;

    public LayerMask stickyLayerMask;
    private void Awake()
    {      
        this._cameraTransform = Camera.main.transform;
        this._playerRigidbody = GetComponent<Rigidbody>();        
    }

    private void Update()
    {        
        this._moveDirection = this.GetLatestMoveDirection();

        this._isGroundedThisFrame = this.IsGrounded();

        if (this._isGroundedThisFrame == true)
        {
            this._playerRigidbody.useGravity = false;
        }
        else if (this._stuckJumpNormal == Vector3.zero)
        {
            this._playerRigidbody.useGravity = true;

            if (PlayerControlsManager.instance.jumpInitiated == true)
            {
                if (this.ShouldBufferJump() == true)
                {
                    this._jumpBuffered = true;
                }

                PlayerControlsManager.instance.jumpInitiated = false;
            }
        }
         
        
        if ( this._stuckJumpNormal != Vector3.zero && this.IsStillAttached() == false)
        {
            this.UnstickPlayer();
        }

        //Debug.LogError("Stuck? " + (this._stuckNormal != Vector3.zero) + "Use Gravity?" + this._playerRigidbody.useGravity);
    }

    private void FixedUpdate()
    {
        this.UpdateCameraRotation();

        if (this._stuckJumpNormal == Vector3.zero)
        {
            this.UpdateLateralVelocity();
            this.UpdateVerticalVelocity();
        }
        else
        {
            this.UpdateStuckPosition();
        }
    }

    private float GetClampedXAngle()
    {
        float xRotation = this._cameraTransform.rotation.eulerAngles.x + PlayerControlsManager.instance.lookDelta.x;

        if (xRotation > 180.0f)
        {
            xRotation -= 360.0f;
        }

        xRotation = Mathf.Clamp(xRotation, -this._maxVerticalAngle, this._maxVerticalAngle);

        return xRotation;
    }

    private void UpdateCameraRotation()
    {
        Quaternion yRotation = Quaternion.AngleAxis(PlayerControlsManager.instance.lookDelta.y, Vector3.up);   
        Quaternion xRotation = Quaternion.Euler(this.GetClampedXAngle(), this._cameraTransform.rotation.eulerAngles.y, 0.0f);        

        this._targetRotation = yRotation * xRotation;

        this._cameraTransform.rotation = this._targetRotation;
        
        if (this._cameraSlerpCoroutine != null)
        {
            StopCoroutine(this._cameraSlerpCoroutine);
        }

        this._cameraSlerpCoroutine = StartCoroutine(this.SlerpToTargetRotation());
        
    }

    private IEnumerator SlerpToTargetRotation()
    {
        while (this._cameraTransform.rotation != this._targetRotation)
        {
            this._cameraTransform.rotation = Quaternion.Lerp(this._cameraTransform.rotation, this._targetRotation, this._cameraRotateSpeed * Time.fixedDeltaTime);

            if (this._cameraTransform.rotation.eulerAngles.y > 360)
            {
                this._cameraTransform.rotation = Quaternion.Euler(this._cameraTransform.rotation.eulerAngles.x, (this._cameraTransform.rotation.eulerAngles.y - 360.0f), this._cameraTransform.rotation.eulerAngles.z);
            }
            else if (this._cameraTransform.rotation.eulerAngles.y < -360)
            {
                this._cameraTransform.rotation = Quaternion.Euler(this._cameraTransform.rotation.eulerAngles.x, (this._cameraTransform.rotation.eulerAngles.y + 360.0f), this._cameraTransform.rotation.eulerAngles.z);
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private void UpdateLateralVelocity()
    {
        Vector3 lateralVelocity = this._moveDirection * this._maxMoveVelocity;

        if (this._moveLerpCoroutine != null)
        {
            StopCoroutine(this._moveLerpCoroutine);
        }

        this._moveLerpCoroutine = StartCoroutine(this.LerpToTargetLateralVelocity(lateralVelocity));
    }

    private IEnumerator LerpToTargetLateralVelocity(Vector3 lateralVelocity)
    {
        Vector3 targetVelocity = new Vector3(lateralVelocity.x, this._playerRigidbody.velocity.y, lateralVelocity.z);

        int iteration = 0;

        while (this._playerRigidbody.velocity != targetVelocity)
        {
            this._playerRigidbody.velocity = Vector3.Lerp(this._playerRigidbody.velocity, targetVelocity, this._moveAcceleration * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
            iteration++;
        }
    }

    private void UpdateVerticalVelocity()
    {
        if (this._isGroundedThisFrame == true)
        {
            if (PlayerControlsManager.instance.jumpInitiated == true || this._jumpBuffered == true)
            {
                this._playerRigidbody.useGravity = false;

                this._playerRigidbody.velocity = new Vector3(this._playerRigidbody.velocity.x, this._jumpForce, this._playerRigidbody.velocity.z);
                
                //this._playerRigidbody.AddForce(0.0f, this._jumpForce, 0.0f, ForceMode.Impulse);                
            }

            PlayerControlsManager.instance.jumpInitiated = false;
            this._jumpBuffered = false;
        }
    }

    private Vector3 GetLatestMoveDirection()
    {
        Vector3 returnVector = Vector3.zero;

        if (PlayerControlsManager.instance.IsPressingForward() == true)
        {
            returnVector += new Vector3(this._cameraTransform.forward.x, 0.0f, this._cameraTransform.forward.z);
        }

        if (PlayerControlsManager.instance.IsPressingBack() == true)
        {
            returnVector -= new Vector3(this._cameraTransform.forward.x, 0.0f, this._cameraTransform.forward.z);
        }

        if (PlayerControlsManager.instance.IsPressingLeft() == true)
        {
            returnVector -= this._cameraTransform.right;
        }

        if (PlayerControlsManager.instance.IsPressingRight() == true)
        {
            returnVector += this._cameraTransform.right;
        }

        return returnVector.normalized;
    }

    private bool IsGrounded()
    {
        float sphereCastMagnitude = this._playerFeetCollider.bounds.extents.y + this._isGroundedDistance;
        float sphereRadius = this._playerFeetCollider.bounds.extents.x / 2.0f;
        RaycastHit hitInfo;

        //Debug.DrawLine(this._playerFeetCollider.transform.position, this._playerFeetCollider.transform.position + (Vector3.down * sphereCastMagnitude), Color.red, 1.0f);

        if (Physics.SphereCast(this._playerFeetCollider.transform.position, sphereRadius, Vector3.down, out hitInfo, sphereCastMagnitude) == true)
        {
            if (hitInfo.collider.gameObject.tag == "DirtPlant" || (hitInfo.collider.gameObject.tag == "Plantable" && hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Plant")))
            {
                this._jumpBuffered = false;
                return false;
            }
            else
            {
                return true;
            }            
        }
        
        return false;
    }

    private bool ShouldBufferJump()
    {
        float bufferDistanceCheck = 2.0f;    

        float sphereCastMagnitude = this._playerFeetCollider.bounds.extents.y + bufferDistanceCheck;
        float sphereRadius = this._playerFeetCollider.bounds.extents.x / 2.0f;
        RaycastHit hitInfo;


        if (Physics.SphereCast(this._playerFeetCollider.transform.position, sphereRadius, Vector3.down, out hitInfo, sphereCastMagnitude) == true)
        {
            return true;
        }

        return false;
    }

    private void UpdateStuckPosition()
    {
        //Debug.DrawLine(this.gameObject.transform.position, this.gameObject.transform.position + (this._stuckMoveDirection * 5.0f), Color.red);

        if (PlayerControlsManager.instance.jumpInitiated == true)
        {        
            PlayerControlsManager.instance.jumpInitiated = false;
            this._jumpBuffered = false;

            this._playerRigidbody.useGravity = false;

            /*
            float XForce = this._stuckJumpNormal.x * this._jumpForce * 3.0f;
            float YForce = 0.75f * this._jumpForce;
            float ZForce = this._stuckJumpNormal.z * this._jumpForce * 3.0f;
            */

            Vector3 unstuckJumpVector = new Vector3(Camera.main.transform.forward.x * this._unstickJumpForce, 
                                                Vector3.up.y * this._jumpForce, 
                                                Camera.main.transform.forward.z * this._unstickJumpForce);

            Debug.LogError("Launch Vector: " + unstuckJumpVector);

            this._playerRigidbody.velocity = unstuckJumpVector;

            this.UnstickPlayer();            
        }
        else
        {
            Vector3 stuckVelocity = Vector3.zero;

            float dotProduct = Vector3.Dot(this._stuckMoveDirection, Camera.main.transform.forward);

            if (PlayerControlsManager.instance.IsPressingForward() == true)
            {
                if (dotProduct > 0)
                {
                    stuckVelocity = this._stuckMoveDirection * this._maxMoveVelocity;
                }
                else 
                {
                    stuckVelocity = -this._stuckMoveDirection * this._maxMoveVelocity;
                }
            }
            if (PlayerControlsManager.instance.IsPressingBack() == true)
            {
                if (dotProduct > 0)
                {
                    stuckVelocity = -this._stuckMoveDirection * this._maxMoveVelocity;
                }
                else
                {
                    stuckVelocity = this._stuckMoveDirection * this._maxMoveVelocity;
                }
            }            

            if (this._moveLerpCoroutine != null)
            {
                StopCoroutine(this._moveLerpCoroutine);
            }

            this._moveLerpCoroutine = StartCoroutine(this.LerpToStuckVelocity(stuckVelocity));
        }
    }

    private IEnumerator LerpToStuckVelocity(Vector3 stuckVelocity)
    {
        int iteration = 0;

        while (this._playerRigidbody.velocity != stuckVelocity)
        {
            this._playerRigidbody.velocity = Vector3.Lerp(this._playerRigidbody.velocity, stuckVelocity, this._moveAcceleration * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
            iteration++;
        }

        this._playerRigidbody.velocity = stuckVelocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
    /*    
    if (collision.gameObject.tag == "Plantable" && this._stuckJumpNormal != Vector3.zero)
        {
            this.UnstickPlayer();
        }
    */
        if (collision.gameObject.tag == "DirtPlant" || (collision.gameObject.tag == "Plantable" && collision.gameObject.layer == LayerMask.NameToLayer("Plant")))
        {
            this.GetTopParent(collision.gameObject.transform).GetComponent<PlantController>().DestroyPlant(true);
        }
        else if (collision.gameObject.tag == "Plant")
        {
            Transform plantParent = this.GetTopParent(collision.gameObject.transform);
        
            if (plantParent.gameObject.GetComponent<PlantController>().isSticky == true)
            {
                this.StickPlayer(plantParent, collision);
            }
        }
    }

    /*
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Plant")
        {
            Transform plantParent = this.GetTopParent(collision.gameObject.transform);

            if (plantParent.gameObject.GetComponent<PlantController>().isSticky == true)
            {
                this.UnstickPlayer();
            }
        }
    }
    */
    private void StickPlayer(Transform parentTransform, Collision collision)
    {
        parentTransform.gameObject.GetComponent<PlantController>().stuckObject = this.gameObject;
        //this.gameObject.transform.parent = parentTransform;
        this._localStuckContactPointOffset = (collision.GetContact(0).point - this.gameObject.transform.position);
        this._stuckJumpNormal = collision.contacts[0].normal;
        this._stuckMoveDirection = collision.gameObject.transform.up;
        this._playerRigidbody.useGravity = false;
        this._playerRigidbody.velocity = Vector3.zero;

        if (this._moveLerpCoroutine != null)
        {
            StopCoroutine(this._moveLerpCoroutine);
        }
    }

    private void UnstickPlayer()
    {
       this.gameObject.transform.parent = null;

        this._stuckJumpNormal = Vector3.zero;

        if (this._moveLerpCoroutine != null)
        {
            StopCoroutine(this._moveLerpCoroutine);
        }
    }

    private Transform GetTopParent(Transform initialTransform)
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

        return currentParent;
    }

    //Raycast in the direction of the normal from the contact point
    private bool IsStillAttached()
    {       
       Vector3 worldStuckContactPoint = this.gameObject.transform.position + (this._localStuckContactPointOffset * 0.8f);

        //Debug.DrawLine(worldStuckContactPoint, worldStuckContactPoint + (-this._stuckJumpNormal * 1.0f), Color.red, 10.0f);


       if (Physics.Raycast(worldStuckContactPoint, -this._stuckJumpNormal, 1.0f, this.stickyLayerMask))
        {
            return true;
        }

        //Instantiate(debugObject, worldStuckContactPoint, new Quaternion());
        //Instantiate(debugObject, worldStuckContactPoint + (-this._stuckJumpNormal * 1.0f), new Quaternion());

        return false;
    }
}
