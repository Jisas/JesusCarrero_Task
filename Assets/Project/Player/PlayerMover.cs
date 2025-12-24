using UnityEngine;
using System;

[RequireComponent(typeof(CharacterController))]
public class PlayerMover : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float speedChangeRate = 10f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSmoothTime = 0.12f;

    [Header("Environment Settings")]
    [SerializeField] private float gravity = -9.81f;

    public event Action<float> OnSpeedChanged;
    public event Action<bool> OnGroundedChanged;

    public CharacterController Controller { get; private set; }
    public float CurrentSpeedNormalized => _currentSpeed / moveSpeed;

    private Transform _mainCameraTransform;
    private readonly float _terminalVelocity = 53f;
    private float _verticalVelocity;
    private float _currentSpeed;
    private float _targetRotation;
    private float _rotationVelocity;

    // Store the calculated address for use in the Update
    private Vector3 _targetDirection;

    private void Start()
    {
        Controller = GetComponent<CharacterController>();

        if (Camera.main != null) 
            _mainCameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        // If there is no input (Idle), we maintain braking and gravity.
        ApplyMovement(_targetDirection, 0f);
    }

    private void ApplyRotation(Vector3 direction)
    {
        // Calculamos el ángulo hacia la dirección de movimiento
        _targetRotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation,
            ref _rotationVelocity, rotationSmoothTime);

        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
    }

    private void ApplyMovement(Vector3 moveDir, float inputMagnitude)
    {
        // 1. Horizontal velocity
        float targetSpeed = (moveDir == Vector3.zero) ? 0f : moveSpeed * inputMagnitude;

        if (Mathf.Abs(_currentSpeed - targetSpeed) < 0.01f) 
             _currentSpeed = targetSpeed;
        else _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, Time.deltaTime * speedChangeRate);

        // 2. Gravity
        if (Controller.isGrounded)
        {
            if (_verticalVelocity < 0.0f) _verticalVelocity = -2f;
        }
        else
        {
            _verticalVelocity += gravity * Time.deltaTime;
            if (_verticalVelocity < -_terminalVelocity) _verticalVelocity = -_terminalVelocity;
        }

        // 3. Apply Movement
        Vector3 finalMotion = (moveDir.normalized * _currentSpeed) + (Vector3.up * _verticalVelocity);

        Controller.Move(finalMotion * Time.deltaTime);

        // 4. Notify
        OnSpeedChanged?.Invoke(CurrentSpeedNormalized);
        OnGroundedChanged?.Invoke(Controller.isGrounded);
    }

    public void Move(Vector2 inputDirection, float inputMagnitude)
    {
        if (_mainCameraTransform == null) return;

        // 1. Calculate the direction relative to the camera
        Vector3 forward = _mainCameraTransform.forward;
        Vector3 right = _mainCameraTransform.right;

        // "Aplanamos" los vectores para que el personaje no intente volar/enterrarse
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Final address in the real world
        _targetDirection = forward * inputDirection.y + right * inputDirection.x;

        if (inputDirection != Vector2.zero)
        {
            ApplyRotation(_targetDirection);
            ApplyMovement(_targetDirection, inputMagnitude);
        }
        else
        {
            _targetDirection = Vector3.zero;
        }
    }
}