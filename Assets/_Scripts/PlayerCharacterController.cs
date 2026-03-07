using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacterController : MonoBehaviour
{
    private Transform _cameraTransform;
    private Rigidbody _playerRigidbody;
    private Collider _playerCollider;

    private Vector3 _moveDirection = Vector3.zero;

    private float _moveAcceleration = 50.0f;
    private float _maxMoveVelocity = 20.0f;
    private float _maxFallVelocity = 300.0f;
    private float _maxVerticalAngle = 85.0f;

    private float _jumpForce = 20.0f;

    private bool _jumpBuffered = false;

    private Quaternion _targetRotation = Quaternion.identity;
    [SerializeField]
    private float _cameraRotateSpeed = 5.0f;
    private Coroutine _cameraSlerpCoroutine = null;

    private void Awake()
    {      
        this._cameraTransform = Camera.main.transform;
        this._playerRigidbody = GetComponent<Rigidbody>();
        this._playerCollider = GetComponentInChildren<Collider>();
    }

    private void Update()
    {        
        this._moveDirection = this.GetLatestMoveDirection();

        if (this.IsGrounded() == true)
        {
            this._playerRigidbody.useGravity = false;
            if (this._jumpBuffered == true)
            {
                PlayerControlsManager.instance.jumpInitiated = true;
                this._jumpBuffered = false;
            }
        }
        else
        {
            this._playerRigidbody.useGravity = true;
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

        if (this._cameraSlerpCoroutine != null)
        {
            StopCoroutine(this._cameraSlerpCoroutine);
        }

        this._cameraSlerpCoroutine = StartCoroutine(this.SlerpToTargetRotation());

        //this._cameraTransform.transform.Rotate(Vector3.up, PlayerControlsManager.instance.lookDelta.y, Space.World);

    }

    private IEnumerator SlerpToTargetRotation()
    {
        while (this._cameraTransform.rotation != this._targetRotation)
        {
            this._cameraTransform.rotation = Quaternion.Slerp(this._cameraTransform.rotation, this._targetRotation, this._cameraRotateSpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
    }

    private void UpdateLateralVelocity()
    {
        Vector3 lateralVelocity = this._moveDirection * this._maxMoveVelocity;

        this._playerRigidbody.velocity = new Vector3(lateralVelocity.x, this._playerRigidbody.velocity.y, lateralVelocity.z);
    }

    private void UpdateVerticalVelocity()
    {
        if (this.IsGrounded())
        {
            this._playerRigidbody.velocity = new Vector3(this._playerRigidbody.velocity.x, 0.0f, this._playerRigidbody.velocity.z);

            if (PlayerControlsManager.instance.jumpInitiated == true)
            {
                this._playerRigidbody.useGravity = false;
                this._playerRigidbody.AddForce(0.0f, this._jumpForce, 0.0f, ForceMode.Impulse);
                PlayerControlsManager.instance.jumpInitiated = false;
            }
        }
        else if (PlayerControlsManager.instance.jumpInitiated == true)
        {
            PlayerControlsManager.instance.jumpInitiated = false;

            if (this.ShouldBufferJump() == true)
            {
                this._jumpBuffered = true;
            }
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
        float raycastMagnitude = this._playerCollider.bounds.extents.y + 0.1f;
        float radius = this._playerCollider.bounds.extents.x - 0.1f;
        RaycastHit hitInfo;

        if (Physics.SphereCast(this._playerCollider.transform.position, radius, Vector3.down, out hitInfo, raycastMagnitude) == true)
        {
            return true;
        }

        return false;
    }

    private bool ShouldBufferJump()
    {
        float raycastMagnitude = this._playerCollider.bounds.extents.y + 2.0f;
        float radius = this._playerCollider.bounds.extents.x - 0.1f;
        RaycastHit hitInfo;

        if (Physics.SphereCast(this._playerCollider.transform.position, radius, Vector3.down, out hitInfo, raycastMagnitude) == true)
        {
            return true;
        }

        return false;
    }

    /*
    private void CapMaxVelocity()
    {
        //this.CapLateralVelocity();
        //this.CapVerticalVelocity();
    }

    private void CapLateralVelocity()
    {
        Vector2 lateralVelocityVector = new Vector2(this._playerRigidbody.velocity.x, this._playerRigidbody.velocity.z);

        if (lateralVelocityVector.magnitude > this._maxMoveVelocity)
        {
            lateralVelocityVector = lateralVelocityVector.normalized * this._maxMoveVelocity;

            this._playerRigidbody.velocity = new Vector3(lateralVelocityVector.x, this._playerRigidbody.velocity.y, lateralVelocityVector.y);
            Debug.LogError("Capping Lateral Velocity");
        }
    }

    private void CapVerticalVelocity()
    {
        if (this._playerRigidbody.velocity.y < -this._maxFallVelocity)
        {
            this._playerRigidbody.velocity = new Vector3(this._playerRigidbody.velocity.x, -this._maxFallVelocity, this._playerRigidbody.velocity.z);
        }
    }
     */
}
