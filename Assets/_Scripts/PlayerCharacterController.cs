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
    private float _maxFallVelocity = 300.0f;
    private float _maxVerticalAngle = 90.0f;
    private float _jumpForce = 20.0f;
    private bool _jumpBuffered = false;

    private bool _isGroundedThisFrame = false;
    private bool _pressedJumpThisFrame = false;

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
        else
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
    }

    private void FixedUpdate()
    {
        this.UpdateCameraRotation();
        this.UpdateLateralVelocity();
        this.UpdateVerticalVelocity();               
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
            return true;
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
}
