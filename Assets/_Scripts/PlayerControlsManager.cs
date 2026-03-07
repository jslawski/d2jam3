using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlsManager : MonoBehaviour
{
    public static PlayerControlsManager instance;

    private PlayerControls _playerControls;

    [HideInInspector]
    public Vector3 lookDelta = Vector3.zero;
    [HideInInspector]
    public bool jumpInitiated = false;
    [HideInInspector]
    public bool shootInitiated = false;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (instance == null)
        {
            instance = this;
        }

        this._playerControls = new PlayerControls();

        this._playerControls.PlayerMap.Look.performed += this.UpdateLookDirection;
        this._playerControls.PlayerMap.Look.canceled += this.StopLookDirection;

        this._playerControls.PlayerMap.Jump.performed += this.ExecuteJump;
        this._playerControls.PlayerMap.Jump.canceled += this.CancelJump;

        this._playerControls.PlayerMap.Shoot.performed += this.ExecuteShoot;
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
        this.lookDelta = new Vector3(-mouseDelta.y, mouseDelta.x, 0.0f);
    }

    private void StopLookDirection(InputAction.CallbackContext context)
    {
        this.lookDelta = Vector3.zero;
    }

    private void ExecuteJump(InputAction.CallbackContext context)
    {
        this.jumpInitiated = true;
    }

    private void CancelJump(InputAction.CallbackContext context)
    {

    }

    private void ExecuteShoot(InputAction.CallbackContext context)
    {        
        this.shootInitiated = true;
    }

    public bool IsPressingForward()
    {
        return this._playerControls.PlayerMap.Forward.inProgress;
    }

    public bool IsPressingBack()
    {
        return this._playerControls.PlayerMap.Back.inProgress;
    }

    public bool IsPressingLeft()
    {
        return this._playerControls.PlayerMap.Left.inProgress;
    }

    public bool IsPressingRight()
    {
        return this._playerControls.PlayerMap.Right.inProgress;
    }

    public bool IsPressingJump()
    {
        return this._playerControls.PlayerMap.Jump.inProgress;
    }

    public bool IsPressingShoot()
    {
        return this._playerControls.PlayerMap.Shoot.inProgress;
    }
}
