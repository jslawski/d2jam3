using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacterController : MonoBehaviour
{
    private Transform _cameraTransform;
    private Rigidbody _playerRigidbody;
    private Collider _playerCollider;

    private PlayerControls _playerControls;

    private Vector3 _moveDirection = Vector3.zero;

    private float _moveAcceleration = 30.0f;
    private float _maxMoveVelocity = 10.0f;
    private float _maxFallVelocity = 300.0f;
    private float _maxVerticalAngle = 60.0f;

    private Vector3 _lookDelta = Vector3.zero;

    private float _jumpForce = 30.0f;

    private bool _jumpInitiated = false;

    private bool _jumpBuffered = false;

    private void Awake()
    {
        this._playerControls = new PlayerControls();

        this._cameraTransform = Camera.main.transform;
        this._playerRigidbody = GetComponent<Rigidbody>();
        this._playerCollider = GetComponentInChildren<Collider>();

        this._playerControls.PlayerMap.Look.performed += this.UpdateLookDirection;
        this._playerControls.PlayerMap.Look.canceled += this.StopLookDirection;

        this._playerControls.PlayerMap.Jump.performed += this.ExecuteJump;
        this._playerControls.PlayerMap.Jump.canceled += this.CancelJump;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        this._playerControls.Enable();
    }

    private void OnDisable()
    {
        this._playerControls.Disable();
    }

    private void UpdateLookDirection(InputAction.CallbackContext context)
    {
        Vector2 mouseDelta = context.ReadValue<Vector2>();
        this._lookDelta = new Vector3(-mouseDelta.y, mouseDelta.x, 0.0f);
    }

    private void StopLookDirection(InputAction.CallbackContext context)
    { 
        this._lookDelta = Vector3.zero;
    }

    private void ExecuteJump(InputAction.CallbackContext context)
    {
        if (this.IsGrounded() == true)
        {
            this._jumpInitiated = true;
        }
        else if (this.ShouldBufferJump() == true)
        {
            this._jumpBuffered = true;
        }
    }

    private void CancelJump(InputAction.CallbackContext context)
    { 
    
    }    

    private void Update()
    {        
        //float raycastMagnitude = this._playerCollider.bounds.extents.y + 3.0f;
        //Debug.DrawLine(this._playerCollider.transform.position, this._playerCollider.transform.position + (Vector3.down * raycastMagnitude), Color.red, 0.01f);

        this._moveDirection = this.GetLatestMoveDirection();

        if (this.IsGrounded() == true)
        {
            this._playerRigidbody.useGravity = false;
            if (this._jumpBuffered == true)
            {
                this._jumpInitiated = true;
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
        if (this.IsGrounded())
        {
            this._playerRigidbody.velocity = new Vector3(this._playerRigidbody.velocity.x, 0.0f, this._playerRigidbody.velocity.z);
        }
    
        if (this._jumpInitiated == true)
        {
            this._playerRigidbody.useGravity = false;
            this._playerRigidbody.AddForce(0.0f, this._jumpForce, 0.0f, ForceMode.Impulse);
            this._jumpInitiated = false;
        }

        this._playerRigidbody.AddForce(this._moveDirection * this._moveAcceleration, ForceMode.Acceleration);

        this._cameraTransform.transform.Rotate(Vector3.up, this._lookDelta.y, Space.World);

        this._cameraTransform.rotation = Quaternion.Euler(this.GetClampedXAngle(), this._cameraTransform.rotation.eulerAngles.y, 0.0f);

        this.CapMaxVelocity();
    }

    private float GetClampedXAngle()
    {
        float xRotation = this._cameraTransform.rotation.eulerAngles.x + this._lookDelta.x;

        if (xRotation > 180.0f)
        {
            xRotation -= 360.0f;
        }

        xRotation = Mathf.Clamp(xRotation, -this._maxVerticalAngle, this._maxVerticalAngle);

        return xRotation;
    }

    private void CapMaxVelocity()
    {
        this.CapLateralVelocity();
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

    private Vector3 GetLatestMoveDirection()
    {
        Vector3 returnVector = Vector3.zero;

        if (this._playerControls.PlayerMap.Forward.inProgress == true)
        {
            returnVector += new Vector3(this._cameraTransform.forward.x, 0.0f, this._cameraTransform.forward.z);
        }

        if (this._playerControls.PlayerMap.Back.inProgress == true)
        {
            returnVector -= new Vector3(this._cameraTransform.forward.x, 0.0f, this._cameraTransform.forward.z);
        }

        if (this._playerControls.PlayerMap.Left.inProgress == true)
        {
            returnVector -= this._cameraTransform.right;
        }

        if (this._playerControls.PlayerMap.Right.inProgress == true)
        {
            returnVector += this._cameraTransform.right;
        }

        return returnVector.normalized;
    }

    private bool IsGrounded()
    {
        float raycastMagnitude = this._playerCollider.bounds.extents.y + 0.1f;

        if (Physics.Raycast(this._playerCollider.transform.position, Vector3.down, raycastMagnitude) == true)
        {
            return true;
        }

        return false;
    }

    private bool ShouldBufferJump()
    {
        float raycastMagnitude = this._playerCollider.bounds.extents.y + 2.0f;

        if (Physics.Raycast(this._playerCollider.transform.position, Vector3.down, raycastMagnitude) == true)
        {
            //Debug.LogError("Buffering Jump!");
            return true;
        }

        return false;
    }
}
