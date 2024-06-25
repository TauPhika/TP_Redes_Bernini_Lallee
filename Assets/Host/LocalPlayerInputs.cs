using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerInputs : MonoBehaviour
{
    //NetworkInputData _inputData;
    //PlayerController _controller;
    
    //float _movementX;
    //float _movementY;
    //bool _isJumpPressed;
    //bool _isDashPressed;
    //bool _isJetpackPressed;
    //bool _isRechargingJetpack;

    //private void Awake()
    //{
    //    _inputData = new NetworkInputData();
    //}

    //public NetworkInputData GetLocalInputs()
    //{
    //    _inputData.movementX = _movementX;
    //    _inputData.movementY = _movementY;
    //    _inputData.isAirborne = _controller.model.isAirborne;
    //    _inputData.isDashPressed = _isDashPressed;
    //    _inputData.isJetpackPressed = _isJetpackPressed;
    //    _inputData.isJumpPressed = _isJumpPressed;
    //    _inputData.isFirePressed = _controller.weapon.FiringInput();
    //    _inputData.isFiring = _controller.weapon._isFiring;
    //    _inputData.isRechargingJetpack = _controller.model.isRechargingJetpack;

    //    _isDashPressed = _isJetpackPressed = _isJumpPressed = false;

    //    return _inputData;
    //}

    //void Update()
    //{
    //    if (!_controller.model._dying)
    //    {

    //        _controller.model.transform.position += new Vector3(_controller.GetMovementX(_controller.model.speed),
    //                                                _controller.GetMovementY(_controller.model.jumpHeight, _controller.model.jetpackPower),
    //                                                0) * Time.deltaTime;

    //        _controller.model.transform.rotation = _controller.GetAimingRotation();

    //        if (_controller.weapon.FiringInput() && !_controller.weapon._isFiring)
    //        {
    //            StartCoroutine(_controller.weapon.FireWeapon());
    //        }

    //        _controller.CheckForDash(_controller.model.dashForce);
    //    }
    //}
}
