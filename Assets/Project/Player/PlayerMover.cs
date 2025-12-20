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

    // Optional: An event for anyone who wants to listen
    public event Action<float> OnSpeedChanged;
    public event Action<bool> OnGroundedChanged;

    // References and Internal Status
    public CharacterController Controller { get; private set; }
    public float CurrentSpeedNormalized => _currentSpeed / moveSpeed;

    private readonly float _terminalVelocity = 53f;
    private float _verticalVelocity;
    private float _currentSpeed;
    private float _targetRotation;
    private float _rotationVelocity;

    private void Start()
    {
        Controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        ApplyMovement(Vector2.zero, 0f);
    }

    public void Move(Vector2 inputDirection, float inputMagnitude)
    {
        ApplyRotation(inputDirection);
        ApplyMovement(inputDirection, inputMagnitude);
    }

    private void ApplyRotation(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        }

        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation,
            ref _rotationVelocity, rotationSmoothTime);

        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
    }

    private void ApplyMovement(Vector2 direction, float inputMagnitude)
    {
        // 1. Calculation of horizontal velocity (Lerp with snap to 0)
        float targetSpeed = (direction == Vector2.zero) ? 0f : moveSpeed * inputMagnitude;

        if (Mathf.Abs(_currentSpeed - targetSpeed) < 0.01f) _currentSpeed = targetSpeed;
        else _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, Time.deltaTime * speedChangeRate);

        // 2. Gravity (Independent of input)
        if (Controller.isGrounded)
        {
            if (_verticalVelocity < 0.0f) _verticalVelocity = -2f;
        }
        else
        {
            _verticalVelocity += gravity * Time.deltaTime;

            if (_verticalVelocity < -_terminalVelocity) 
                _verticalVelocity = -_terminalVelocity;
        }

        // 3. Apply Movement
        Vector3 moveDir = new Vector3(direction.x, 0, direction.y).normalized;
        Vector3 finalMotion = (moveDir * _currentSpeed) + (Vector3.up * _verticalVelocity);

        Controller.Move(finalMotion * Time.deltaTime);

        // 4. Notify changes
        OnSpeedChanged?.Invoke(CurrentSpeedNormalized);
        OnGroundedChanged?.Invoke(Controller.isGrounded);
    }
}