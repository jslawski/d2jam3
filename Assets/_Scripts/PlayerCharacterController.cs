using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacterController : MonoBehaviour
{
    private Transform _cameraTransform;
    private Rigidbody _playerRigidbody;    

    private float _currentVerticalSpeed = 0.0f;
    private float _terminalVelocity = 30.0f;

    private PlayerControls _playerControls;

    private Vector3 _moveDirection = Vector3.zero;

    private float _moveAcceleration = 30.0f;
    private float _maxMoveVelocity = 50.0f;
    private float _maxFallVelocity = 30.0f;

    private void Awake()
    {
        this._playerControls = new PlayerControls();

        this._cameraTransform = Camera.main.transform;
        this._playerRigidbody = GetComponent<Rigidbody>();

        this.SetupPlayerControls();
    }

    private void OnEnable()
    {
        this._playerControls.Enable();
    }

    private void OnDisable()
    {
        this._playerControls.Disable();
    }

    private void SetupPlayerControls()
    {
        /*
        this._playerControls.PlayerMap.Left.performed += this.MoveLeft;        
        this._playerControls.PlayerMap.Right.performed += this.MoveRight;
        this._playerControls.PlayerMap.Forward.performed += this.MoveForward;
        this._playerControls.PlayerMap.Back.performed += this.MoveBack;
        this._playerControls.PlayerMap.Left.canceled += this.StopDirection;
        this._playerControls.PlayerMap.Right.canceled += this.StopDirection;
        this._playerControls.PlayerMap.Forward.canceled += this.StopDirection;
        this._playerControls.PlayerMap.Back.canceled += this.StopDirection;
        */

    }

    private void MoveLeft(InputAction.CallbackContext context)
    {
        this._moveDirection = new Vector3(-this._cameraTransform.right.x, this._moveDirection.y, this._moveDirection.z);
        Debug.LogError("Right: " + this._cameraTransform.right);
    }

    private void MoveRight(InputAction.CallbackContext context)
    {
        this._moveDirection = new Vector3(this._cameraTransform.right.x, this._moveDirection.y, this._moveDirection.z);
    }

    private void MoveForward(InputAction.CallbackContext context)
    {
        this._moveDirection = new Vector3(this._moveDirection.x, this._moveDirection.y, this._cameraTransform.forward.z);
    }
    private void MoveBack(InputAction.CallbackContext context)
    {
        this._moveDirection = new Vector3(this._moveDirection.x, this._moveDirection.y, -this._cameraTransform.forward.z);
    }

    private void StopDirection(InputAction.CallbackContext context)
    {
        this.StopHorizontalMovement();
        this.StopForwardBackMovement();
    }

    private void StopHorizontalMovement()
    {
        if (this._playerControls.PlayerMap.Left.inProgress == false && this._playerControls.PlayerMap.Right.inProgress == false)
        {
            this._moveDirection = new Vector3(0.0f, this._moveDirection.y, this._moveDirection.z);
        }
        else if (this._playerControls.PlayerMap.Left.inProgress == true)
        {
            this._moveDirection = new Vector3(-this._cameraTransform.right.x, this._moveDirection.y, this._moveDirection.z);
        }
        else if (this._playerControls.PlayerMap.Right.inProgress == true)
        {
            this._moveDirection = new Vector3(this._cameraTransform.right.x, this._moveDirection.y, this._moveDirection.z);
        }
    }

    private void StopForwardBackMovement()
    {
        if (this._playerControls.PlayerMap.Forward.inProgress == false && this._playerControls.PlayerMap.Back.inProgress == false)
        {
            this._moveDirection = new Vector3(this._moveDirection.x, this._moveDirection.y, 0.0f);
        }
        else if (this._playerControls.PlayerMap.Forward.inProgress == true)
        {
            this._moveDirection = new Vector3(this._moveDirection.x, this._moveDirection.y, this._cameraTransform.forward.z);
        }
        else if (this._playerControls.PlayerMap.Back.inProgress == true)
        {
            this._moveDirection = new Vector3(this._moveDirection.x, this._moveDirection.y, -this._cameraTransform.forward.z);
        }
    }

    private void Update()
    {
        this._moveDirection = this.GetLatestMoveDirection();
    }

    private void FixedUpdate()
    {
        this._playerRigidbody.AddForce(this._moveDirection * this._moveAcceleration, ForceMode.Acceleration);

        this.CapMaxVelocity();
    }

    private void CapMaxVelocity()
    {
        this.CapLateralVelocity();
        this.CapVerticalVelocity();
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

    /*
    private void ApplyGravity()
    {
        if (this._characterController.isGrounded)
        {
            this._currentVerticalSpeed = 0.0f;
        }

        float accelerationAmountPerFrame = Physics.gravity.y * Time.deltaTime;

        this._currentVerticalSpeed -= accelerationAmountPerFrame;

        if (this._currentVerticalSpeed > (this._terminalVelocity * Time.deltaTime))
        {
            this._currentVerticalSpeed = (this._terminalVelocity * Time.deltaTime);
        }

        this._characterController.Move(Vector3.down * this._currentVerticalSpeed);
    }
    */
}
