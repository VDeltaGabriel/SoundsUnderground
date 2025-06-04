using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class Player : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 0.7f;
    [SerializeField] private float _rotSpeed = 0.7f;
    private PlayerActions _playerActions;
    private float _moveDir;
    private float _rotDir;
    private Rigidbody _rb;
    private Sonar _sonar;

    public PlayerActions PlayerActions => _playerActions;

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }
    
    private void Awake()
    {
        _playerActions = new PlayerActions();
        _rb = GetComponent<Rigidbody>();
        _sonar = GetComponent<Sonar>();
    }

    private void OnEnable()
    {
        _playerActions.Enable();
        _playerActions.Actions.move.performed += Move;
        _playerActions.Actions.rotate.performed += Rot;
        _playerActions.Actions.sonar.performed += _sonar.SendSonar;
    }

    private void OnDisable()
    {
        _playerActions.Disable();
        _playerActions.Actions.move.performed -= Move;
        _playerActions.Actions.rotate.performed -= Rot;
        _playerActions.Actions.sonar.performed -= _sonar.SendSonar;
    }

    private void Move(InputAction.CallbackContext ctx)
    {
        _moveDir = ctx.ReadValue<float>();
    }

    private void Rot(InputAction.CallbackContext ctx)
    {
        _rotDir = ctx.ReadValue<float>();
    }

    private void Update()
    {
        _rb.velocity = transform.forward * (_moveDir * _moveSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up, 1.0f * _rotDir * _rotSpeed * Time.deltaTime);
    }
}
